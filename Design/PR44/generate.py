from pathlib import Path
import xml.etree.ElementTree as E
import json

OUT=Path(__file__).parent
OUT.mkdir(exist_ok=True)
INK='#1F2937'; MUTED='#64748B'; LINE='#CBD5E1'; BG='#F5F7FA'; RED='#B42318'
SCREENS=[('home','Головна'),('list','Список відправлень'),('new','Нове відправлення'),('errors','Помилки форми'),('edit','Редагування'),('details','Деталі відправлення'),('about','Про програму'),('delete','Підтвердження видалення'),('deleted','Після видалення')]
VARIANTS=[('desktop','Windows Forms',1280,900),('web','Web Desktop',1440,960),('mobile','Web Mobile',390,900)]
IDS={f'{v}-{s}':f'{v}-{s}' for v,_,_,_ in VARIANTS for s,_ in SCREENS}
manifest=[]; links=[]; pages=[]

class Page:
    def __init__(self,key,name,w,h):
        self.key=key;self.name=name;self.w=w;self.h=h;self.i=1
        self.diagram=E.Element('diagram',id=key,name=name)
        self.model=E.SubElement(self.diagram,'mxGraphModel',dx=str(w),dy=str(h),grid='0',gridSize='10',guides='1',tooltips='1',connect='1',arrows='1',fold='1',page='1',pageScale='1',pageWidth=str(w),pageHeight=str(h),math='0',shadow='0',background='#FFFFFF')
        self.root=E.SubElement(self.model,'root');E.SubElement(self.root,'mxCell',id='0');E.SubElement(self.root,'mxCell',id='1',parent='0')
        self.rect(0,0,w,h,'',fill=BG,stroke='none')

    def rect(self,x,y,w,h,text='',fill='#FFFFFF',stroke=LINE,size=16,color=INK,bold=False,align='left',round=False,link=None,style=''):
        assert x>=0 and y>=0 and x+w<=self.w+1 and y+h<=self.h+1,(self.key,text,x,y,w,h)
        self.i+=1; ident=f'{self.key}-c{self.i}'
        parent=self.root
        if link:
            assert link in IDS,link
            parent=E.SubElement(self.root,'object',id=ident,label=text,link='data:page/id,'+IDS[link])
            links.append((self.key,link,text))
            attrs={}
        else:attrs={'id':ident,'value':text}
        attrs.update(vertex='1',parent='1',style=f'rounded={int(round)};arcSize=12;whiteSpace=wrap;html=0;fillColor={fill};strokeColor={stroke};fontFamily=Segoe UI;fontSize={size};fontColor={color};fontStyle={1 if bold else 0};align={align};verticalAlign=middle;spacingLeft={12 if align=="left" else 0};spacingRight=8;spacingTop=2;spacingBottom=2;{style}')
        cell=E.SubElement(parent,'mxCell',attrs)
        E.SubElement(cell,'mxGeometry',x=str(x),y=str(y),width=str(w),height=str(h),**{'as':'geometry'})
        return ident

    def text(self,x,y,w,h,text,size=16,color=INK,bold=False,align='left',link=None):
        return self.rect(x,y,w,h,text,fill='none',stroke='none',size=size,color=color,bold=bold,align=align,link=link)

    def btn(self,x,y,w,text,target,kind='secondary',h=42):
        fill=self.accent if kind=='primary' else RED if kind=='danger' else '#FFFFFF'
        color='#FFFFFF' if kind in ['primary','danger'] else INK
        return self.rect(x,y,w,h,text,fill=fill,stroke=fill if kind!='secondary' else LINE,color=color,size=15,bold=kind!='secondary',round=self.variant!='desktop',align='center',link=target)

    def alert(self,x,y,w,text):
        self.rect(x,y,w,60,fill='#FEF3F2',stroke='#FDA29B',round=True)
        self.rect(x+12,y+17,26,26,'!',fill='#FEE4E2',stroke=RED,color=RED,bold=True,align='center',style='shape=ellipse;')
        self.text(x+46,y+5,w-58,50,text,size=14,color=RED,bold=True)

def shell(variant,screen):
    key=f'{variant}-{screen}'; title=dict(SCREENS)[screen]
    _,vn,w,h=next(v for v in VARIANTS if v[0]==variant)
    if variant=='mobile': h=1580 if screen in ['new','edit'] else 1740 if screen=='errors' else 1220 if screen=='details' else 1060 if screen in ['list','deleted','delete'] else 900
    p=Page(key,f'{vn} — {title}',w,h);p.variant=variant;p.accent='#0078D4' if variant=='desktop' else '#0F766E'
    prefix=variant+'-'
    if variant=='desktop':
        p.rect(0,0,w,36,'PostService — '+title,fill='#E6EBF1',stroke='none',size=14)
        p.text(w-120,0,110,36,'—    □    ×',size=15,align='right')
        p.rect(0,36,w,48,fill='#FFFFFF',stroke='none')
        for x,label,s in [(20,'Головна','home'),(160,'Відправлення','list'),(360,'Про програму','about')]:
            active=s==screen or s=='list' and screen in ['new','errors','edit','details','delete','deleted']
            p.text(x,42,180 if s=='list' else 140,36,label,size=15,bold=active,color=p.accent if active else INK,link=prefix+s)
        p.x=44;p.content=1192;p.y=108
    elif variant=='web':
        p.rect(0,0,230,h,fill='#17343B',stroke='none')
        p.text(20,30,190,40,'PostService',size=26,color='#FFFFFF',bold=True,link=prefix+'home')
        p.text(20,72,190,36,'Поштові відправлення',size=14,color='#C8DCDC')
        for y,label,s in [(150,'Головна','home'),(212,'Відправлення','list'),(274,'Про програму','about')]:
            active=s==screen or s=='list' and screen in ['new','errors','edit','details','delete','deleted']
            p.rect(18,y,194,46,label,fill='#28515A' if active else 'none',stroke='none',color='#FFFFFF',size=17,bold=active,round=True,link=prefix+s)
        p.text(20,h-65,190,40,'Робоче місце оператора',size=13,color='#C8DCDC')
        p.rect(230,0,w-230,72,'Облік відправлень',fill='#FFFFFF',stroke='none',size=16,color=MUTED)
        p.text(w-240,18,200,36,'Поштова служба',size=14,color=MUTED,align='right')
        p.x=270;p.content=1130;p.y=104
    else:
        p.rect(0,0,w,32,'9:41                         4G     100%',fill='#17343B',stroke='none',color='#FFFFFF',size=12)
        p.rect(0,32,w,62,'PostService',fill='#FFFFFF',stroke='none',size=22,bold=True,link=prefix+'home')
        for x,label,s in [(16,'Головна','home'),(133,'Відправлення','list'),(250,'Про програму','about')]:
            active=s==screen or s=='list' and screen in ['new','errors','edit','details','delete','deleted']
            p.rect(x,102,112,44,label,fill='#DDF2ED' if active else '#FFFFFF',stroke=LINE,color=p.accent if active else INK,size=13,bold=active,round=True,align='center',link=prefix+s)
        p.x=16;p.content=358;p.y=170
    p.text(p.x,p.y,p.content,44,title if screen!='errors' else 'Нове відправлення',size=26 if variant!='mobile' else 24,bold=True)
    p.y+=60
    return p

def home(p):
    x,w,y=p.x,p.content,p.y;pre=p.variant+'-';mobile=p.variant=='mobile'
    p.rect(x,y,w,270 if mobile else 280,fill='#FFFFFF',round=True)
    p.text(x+20,y+24,w-40,80 if mobile else 54,'Усі відправлення\nв одному місці' if mobile else 'Усі відправлення в одному місці',size=28 if mobile else 34,bold=True)
    p.text(x+20,y+(112 if mobile else 95),w-40,70,'Переглядайте відправлення, створюйте нові та перевіряйте їхні деталі.',size=17,color=MUTED)
    p.btn(x+20,y+(202 if mobile else 190),w-40 if mobile else 290,'До списку відправлень',pre+'list','primary',h=48)
    yy=y+(302 if mobile else 320)
    if mobile:
        p.btn(x,yy,w,'Нове відправлення',pre+'new',h=48)
        p.btn(x,yy+66,w,'Про програму',pre+'about',h=48)
    else:
        for xx,heading,body,target in [(x,'3 відправлення','Переглянути поточний список','list'),(x+405,'Нове відправлення','Внести дані відправлення','new')]:
            p.rect(xx,yy,370,140,fill='#FFFFFF',round=True,link=pre+target)
            p.text(xx+16,yy+20,338,45,heading,size=24,bold=True)
            p.text(xx+16,yy+74,338,40,body,size=16,color=MUTED)
        p.btn(x,yy+170,210,'Про програму',pre+'about')

rows=[('1042','Олена Коваленко','Андрій Мельник','Книги','Кур’єром','2,5','97,50'),('1043','Іван Петренко','Марія Бондар','Документи','До відділення','0,5','45,00'),('1044','Софія Шевченко','Максим Коваль','Подарунок','Експрес-кур’єром','1,2','130,00')]

def posting_list(p,deleted=False):
    x,w,y=p.x,p.content,p.y;pre=p.variant+'-';mobile=p.variant=='mobile'
    if deleted:
        p.rect(x,y,w,48,'✓  Відправлення №1042 видалено.',fill='#E6F4EA',stroke='#A9D7B4',color='#175C2E',size=15,round=True);y+=66
    if mobile:
        p.btn(x,y,w,'+ Нове відправлення',pre+'new','primary',h=48);y+=64
        p.text(x,y,w,30,('2 відправлення • жодного не вибрано' if deleted else '3 відправлення • вибрано 1'),size=14,color=MUTED);y+=42
        display=rows[1:] if deleted else rows
        for index,row in enumerate(display):
            selected=index==0 and not deleted
            p.rect(x,y,w,156,fill='#E9F5F1' if selected else '#FFFFFF',stroke=p.accent if selected else LINE,round=True,style='strokeWidth=2;' if selected else '')
            p.text(x+10,y+8,w-20,30,'№'+row[0]+('   ✓ Вибрано' if selected else ''),size=16,color=p.accent if selected else MUTED,bold=selected)
            p.text(x+10,y+42,w-20,30,row[1]+' →',size=16,bold=True)
            p.text(x+10,y+73,w-20,30,row[2],size=16,bold=True)
            p.text(x+10,y+111,w-20,34,f'{row[3]} • {row[5]} кг • {row[6]} грн',size=15)
            y+=170
        if not deleted:
            p.btn(x,y,w,'Деталі',pre+'details',h=46);y+=58
            p.btn(x,y,w,'Редагувати',pre+'edit',h=46);y+=58
            p.btn(x,y,w,'Видалити',pre+'delete','danger',h=46)
        else:p.btn(x,y,w,'Оновити',pre+'deleted',h=46)
    else:
        p.btn(x,y,210,'+ Нове відправлення',pre+'new','primary')
        for bx,bw,label,target in [(x+226,165,'Редагувати','edit'),(x+407,130,'Деталі','details'),(x+553,140,'Видалити','delete')]:
            if deleted:p.rect(bx,y,bw,42,label,fill='#E9EDF1',color='#7D8793',align='center',size=15)
            else:p.btn(bx,y,bw,label,pre+target,'danger' if target=='delete' else 'secondary')
        p.btn(x+w-140,y,140,'Оновити',pre+('deleted' if deleted else 'list'));y+=64
        p.text(x,y,w,30,('2 відправлення • жодного не вибрано' if deleted else '3 відправлення • вибрано 1'),size=15,color=MUTED);y+=42
        headings=['№','Відправник','Одержувач','Вміст','Доставка','Вага, кг','Ціна, грн']
        proportions=[.075,.18,.18,.15,.19,.09,.135]
        xx=x
        for title,f in zip(headings,proportions):p.rect(xx,y,w*f,48,title,fill='#E7EDF3',size=14,bold=True);xx+=w*f
        y+=48
        display=rows[1:] if deleted else rows
        for index,row in enumerate(display):
            selected=index==0 and not deleted;xx=x
            fill=('#0078D4' if p.variant=='desktop' else '#DDF2ED') if selected else '#FFFFFF'
            color='#FFFFFF' if selected and p.variant=='desktop' else INK
            for val,f in zip(row,proportions):p.rect(xx,y,w*f,62,val,fill=fill,color=color,size=14,bold=selected);xx+=w*f
            y+=62
        p.text(x,y+20,w,45,'Виберіть відправлення, щоб відкрити деталі, редагувати або видалити його.' if deleted else '✓ Вибрано відправлення №'+display[0][0]+'. Дії застосовуються до цього рядка.',size=15,color=MUTED)

def form(p,error=False,edit=False):
    x,w,y=p.x,p.content,p.y;pre=p.variant+'-';mobile=p.variant=='mobile'
    if error:
        p.alert(x,y,w,'Не вдалося зберегти. Перевірте: відправник, вага й оголошена цінність.');y+=80
    values={'from':'Олена Коваленко, Львів','to':'Андрій Мельник, Київ','content':'Книги','delivery':'Кур’єром','weight':'2,5','value':'250,00','description':'Крихке','price':'97,50'}
    if not edit and not error:values={'from':'Введіть ПІБ та адресу','to':'Введіть ПІБ та адресу','content':'Що містить відправлення?','delivery':'До відділення','weight':'Вкажіть вагу','value':'Не вказано','description':'За потреби','price':'Буде розраховано'}
    if error:values.update({'from':'Не заповнено','weight':'0','value':'−20,00','price':'Буде розраховано'})
    errors={'from':'Вкажіть ПІБ та адресу відправника.','weight':'Вкажіть вагу, більшу за 0 кг.','value':'Вкажіть цінність від 0 грн або залиште порожньою.'}
    fields=[('from','Відправник *'),('to','Одержувач *'),('content','Вміст *'),('delivery','Тип доставки *'),('weight','Вага, кг *'),('dimensions','Розміри, см *'),('value','Оголошена цінність, грн'),('description','Опис'),('price','Вартість доставки, грн')]
    for name,label in fields:
        invalid=error and name in errors
        if mobile:
            p.text(x,y,w,28,label,size=15,bold=True);y+=34
            if name=='dimensions':
                for offset,sub,val in [(0,'Ширина','30'),(122,'Висота','20'),(244,'Глибина','10')]:
                    p.text(x+offset,y,114,25,sub,size=13,color=MUTED)
                    p.rect(x+offset,y+28,114,44,val if edit or error else '—',size=16,round=True)
                y+=88;continue
            readonly=name=='price' and not edit
            p.rect(x,y,w,48,values[name]+('  ▾' if name=='delivery' else ''),fill='#F0F3F6' if readonly else '#FFF4F3' if invalid else '#FFFFFF',stroke=RED if invalid else LINE,color=MUTED if readonly else INK,size=16,round=True)
            y+=54
            if invalid:
                p.rect(x,y+2,22,22,'!',fill='#FEE4E2',stroke=RED,color=RED,bold=True,align='center',style='shape=ellipse;')
                p.text(x+28,y,w-28,46,errors[name],size=13,color=RED);y+=54
            y+=20
        else:
            p.text(x,y,232,42,label,size=15,bold=True)
            fieldx=x+242;fw=450 if p.variant=='desktop' else 420
            if name=='dimensions':
                for offset,sub,val in [(0,'Ширина','30'),(150,'Висота','20'),(300,'Глибина','10')]:
                    p.rect(fieldx+offset,y,140,42,sub+': '+(val if edit or error else '—'),size=14)
            else:
                readonly=name=='price' and not edit
                p.rect(fieldx,y,fw,42,values[name]+('  ▾' if name=='delivery' else ''),fill='#F0F3F6' if readonly else '#FFF4F3' if invalid else '#FFFFFF',stroke=RED if invalid else LINE,color=MUTED if readonly else INK,size=15,round=p.variant=='web')
                if invalid:
                    ex=fieldx+fw+16
                    p.rect(ex,y+9,24,24,'!',fill='#FEE4E2',stroke=RED,color=RED,bold=True,align='center',style='shape=ellipse;')
                    p.text(ex+32,y-3,w-(ex-x)-32,50,errors[name],size=14,color=RED)
            y+=58
    if mobile:
        if not edit:p.text(x,y,w,48,'Вартість доставки визначається під час збереження.',size=13,color=MUTED);y+=62
        p.btn(x,y,w,'Зберегти',pre+('errors' if not error and not edit else 'list'),'primary',h=48)
        p.btn(x,y+62,w,'Скасувати',pre+'list',h=48)
    else:
        if not edit:p.text(x+242,y,450,38,'Вартість доставки визначається під час збереження.',size=13,color=MUTED);y+=48
        p.btn(x+242,y,180,'Зберегти',pre+('errors' if not error and not edit else 'list'),'primary')
        p.btn(x+438,y,180,'Скасувати',pre+'list')

DETAILS=[('Номер','1042'),('Відправник','Олена Коваленко, Львів'),('Одержувач','Андрій Мельник, Київ'),('Вміст','Книги'),('Тип доставки','Кур’єром'),('Вага','2,5 кг'),('Ширина / висота / глибина','30 / 20 / 10 см'),('Оголошена цінність','250,00 грн'),('Вартість доставки','97,50 грн'),('Опис','Крихке'),('Створено','07.11.2026 15:40')]
def details(p):
    x,w,y=p.x,p.content,p.y;mobile=p.variant=='mobile';pre=p.variant+'-'
    p.text(x,y,w,32,'Відправлення №1042 • перегляд',size=16,color=MUTED);y+=48
    for label,value in DETAILS:
        if mobile:
            p.text(x,y,w,26,label,size=13,color=MUTED);p.text(x,y+28,w,34,value,size=17,bold=True);y+=73
        else:
            p.rect(x,y,w,46,fill='#FFFFFF',stroke=LINE)
            p.text(x+12,y+2,330,42,label,size=15,color=MUTED)
            p.text(x+350,y+2,w-370,42,value,size=16,bold=True);y+=46
    y+=26;p.btn(x,y,w if mobile else 200,'До списку',pre+'list',h=46)

def about(p):
    x,w,y=p.x,p.content,p.y;mobile=p.variant=='mobile';pre=p.variant+'-'
    p.rect(x,y,w,400 if mobile else 340,fill='#FFFFFF',round=True)
    p.text(x+20,y+22,w-40,60,'PostService',size=30,bold=True)
    p.text(x+20,y+88,w-40,40,'Облік поштових відправлень',size=18,bold=True)
    p.text(x+20,y+145,w-40,130 if mobile else 100,'Застосунок допомагає переглядати, створювати, редагувати та видаляти поштові відправлення.\n\nПеред видаленням завжди запитується підтвердження.',size=16,color=MUTED)
    p.text(x+20,y+(310 if mobile else 275),w-40,45,'Версія 1.0',size=15,color=MUTED)
    p.btn(x,y+(430 if mobile else 374),w if mobile else 200,'На головну',pre+'home','primary',h=46)

def delete(p):
    posting_list(p)
    p.rect(0,84 if p.variant=='desktop' else 72 if p.variant=='web' else 146,p.w,p.h-(84 if p.variant=='desktop' else 72 if p.variant=='web' else 146),fill='#CFD5DB',stroke='none',style='opacity=65;')
    mobile=p.variant=='mobile';dw=358 if mobile else 610;dh=370 if mobile else 330
    dx=(p.w-dw)/2;dy=300 if mobile else 280
    p.rect(dx,dy,dw,dh,fill='#FFFFFF',stroke='#98A2B3',round=p.variant!='desktop',style='shadow=1;')
    p.text(dx+18,dy+16,dw-36,44,'Підтвердження видалення',size=21,bold=True)
    p.rect(dx+22,dy+80,34,34,'!',fill='#FEF0C7',stroke='#B7791F',color='#92400E',size=24,bold=True,align='center',style='shape=ellipse;')
    p.text(dx+65,dy+68,dw-84,70,'Видалити відправлення №1042?',size=19,bold=True)
    p.text(dx+22,dy+147,dw-44,65,'Олена Коваленко → Андрій Мельник\nКниги • 2,5 кг • 97,50 грн',size=15)
    p.text(dx+22,dy+220,dw-44,36,'Цю дію неможливо скасувати.',size=14,color=RED)
    if mobile:
        p.btn(dx+18,dy+276,154,'Скасувати',p.variant+'-list',h=48)
        p.btn(dx+186,dy+276,154,'Видалити',p.variant+'-deleted','danger',h=48)
    else:
        p.btn(dx+240,dy+270,160,'Скасувати',p.variant+'-list')
        p.btn(dx+418,dy+270,170,'Видалити',p.variant+'-deleted','danger')

for variant,_,_,_ in VARIANTS:
    for screen,_ in SCREENS:
        p=shell(variant,screen)
        if screen=='home':home(p)
        elif screen in ['list','deleted']:posting_list(p,screen=='deleted')
        elif screen in ['new','errors','edit']:form(p,screen=='errors',screen=='edit')
        elif screen=='details':details(p)
        elif screen=='about':about(p)
        elif screen=='delete':delete(p)
        pages.append(p)
        manifest.append({'id':p.key,'name':p.name,'width':p.w,'height':p.h,'file':f'{variant}/{screen}.png'})

overview=Page('navigation','Навігація та стани макетів',1440,1000)
overview.variant='web';overview.accent='#0F766E'
overview.text(40,30,1360,64,'PostService — навігація та стани макетів',size=32,bold=True)
overview.text(40,100,1360,60,'Кожна картка відкриває відповідну сторінку дизайну. Це прототип переходів, без звернення до API.',size=18,color=MUTED)
for index,(variant,name,_,_) in enumerate(VARIANTS):
    x=40+index*460
    overview.text(x,190,420,50,name,size=25,bold=True)
    for j,(screen,label) in enumerate(SCREENS):overview.btn(x,260+j*65,420,label,f'{variant}-{screen}',h=48)
overview.text(40,880,1360,75,'Основний шлях: Головна → Список → Створення / Деталі / Редагування / Видалення.\nПорожня форма → Помилки; Видалення → Підтвердження → Стан після видалення.',size=18,color=MUTED)
pages.append(overview)
manifest.append({'id':'navigation','name':overview.name,'width':1440,'height':1000,'file':'navigation.png'})
mx=E.Element('mxfile',host='Electron',agent='draw.io',version='32.6.10',type='device',compressed='false')
for p in pages:mx.append(p.diagram)
E.indent(mx,space='  ')
E.ElementTree(mx).write(OUT/'PostService-UI.drawio',encoding='utf-8',xml_declaration=True)
(OUT/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
(OUT/'navigation.json').write_text(json.dumps(links,ensure_ascii=False,indent=2),encoding='utf-8')
for v,_,_,_ in VARIANTS:(OUT/v).mkdir(exist_ok=True)
# Verify reachability: every screen is linked back to its variant's list or home.
for variant,_,_,_ in VARIANTS:
    for screen,_ in SCREENS:
        origin=variant+'-'+screen;seen={origin};queue=[origin]
        while queue:
            current=queue.pop()
            for a,b,_ in links:
                if a==current and b not in seen:seen.add(b);queue.append(b)
        assert variant+'-home' in seen and variant+'-list' in seen,(origin,seen)
assert len(pages)==28
print(f'Created {len(pages)} editable CASE pages, {len(links)} navigation links; all screens can return home and to the list.')
