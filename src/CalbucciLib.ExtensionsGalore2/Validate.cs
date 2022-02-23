using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.ExtensionsGalore2;

public enum DatePartsOrder
{
    /// <summary>
    /// Year/Month/Day
    /// </summary>
    YMD,

    /// <summary>
    /// Day/Month/Year
    /// </summary>
    DMY,

    /// <summary>
    /// Month/Day/Year
    /// </summary>
    MDY
}
public static class Validate
{
    private static HashSet<string> _tlds;

    private static string _validPhoneNumberSymbols = " .-()#_+";
    private static Dictionary<string, string> _usAreaCodes;
    private static HashSet<string> _notValidTwitterAccounts;


    static Validate()
    {
        _notValidTwitterAccounts = new(StringComparer.InvariantCultureIgnoreCase)
        {
            "about",
            "favorites",
            "followers",
            "following",
            "hashtag",
            "i",
            "intent",
            "statuses",
            "status",
            "messages",
            "privacy",
            "search",
            "search-advanced",
            "search-home",
            "settings",
            "signin",
            "signup",
            "tos",
            "who_to_follow",
        };

        // http://www.bennetyee.org/ucsd-pages/area.html
        var allAreaCodesZone1 = new string[]
        {
            "201","202","203","204","205","206","207","208","209","210","211","212","213","214","215","216","217","218","219","224","225","226","228","229","231","234","236","239","240","242","246","248",
            "250","251","252","253","254","256","260","262","264","267","268","269","270","276","278","281","283","284","289",
            "301","302","303","304","305","306","307","308","309","310","311","312","313","314","315","316","317","318","319","320","321","323","325","330","331","334","336","337","339","340","341","345","347",
            "351","352","360","361","369","380","385","386","401","402","403","404","405","406","407","408","409","410","411","412","413","414","415","416","417","418","419","423","424","425","430","432","434","435","438","440","441","442","443",
            "450","456","464","469","470","473","475","478","479","480","484",
            "500","501","502","503","504","505","506","507","508","509","510","511","512","513","514","515","516","517","518","519","520","530","540","541",
            "551","555","557","559","561","562","563","564","567","570","571","573","574","575","580","585","586",
            "600","601","602","603","604","605","606","607","608","609","610","611","612","613","614","615","616","617","618","619","620","623","626","627","628","630","631","636","641","646","647","649",
            "650","651","660","661","662","664","669","670","671","678","679","682","684","689",
            "700","701","702","703","704","705","706","707","708","709","710","711","712","713","714","715","716","717","718","719","720","724","727","731","732","734","737","740","747",
            "754","757","758","760","762","763","764","765","767","769","770","772","773","774","775","778","779","780","781","784","785","786","787",
            "800","801","802","803","804","805","806","807","808","809","810","811","812","813","814","815","816","817","818","819","822","828","829","830","831","832","833","835","843","844","845","847","848",
            "850","855","856","857","858","859","860","862","863","864","865","866","867","868","869","870","872","876","877","878","880","881","882","888","898",
            "900","901","902","903","904","905","906","907","908","909","910","911","912","913","914","915","916","917","918","919","920","925","927","928","931","935","936","937","939","940","941","947","949",
            "951","952","954","956","957","959","970","971","972","973","975","976","978","979","980","984","985","989"
        };
        _usAreaCodes = new Dictionary<string, string>(allAreaCodesZone1.Length);
        foreach (var ac in allAreaCodesZone1)
            _usAreaCodes[ac] = ac;

        // Updated on Feb/22/2022 https://data.iana.org/TLD/tlds-alpha-by-domain.txt
        var tlds = new[]
        {

            "aaa", "aarp", "abarth", "abb", "abbott", "abbvie", "abc", "able", "abogado", "abudhabi", "academy", "accenture",
            "accountant", "accountants", "aco", "actor", "adac", "ads", "adult", "aeg", "aero", "aetna", "afl", "africa", "agakhan",
            "agency", "aig", "airbus", "airforce", "airtel", "akdn", "alfaromeo", "alibaba", "alipay", "allfinanz", "allstate",
            "ally", "alsace", "alstom", "amazon", "americanexpress", "americanfamily", "amex", "amfam", "amica", "amsterdam",
            "analytics", "android", "anquan", "anz", "aol", "apartments", "app", "apple", "aquarelle", "arab", "aramco", "archi",
            "army", "arpa", "art", "arte", "asda", "asia", "associates", "athleta", "attorney", "auction", "audi", "audible",
            "audio", "auspost", "author", "auto", "autos", "avianca", "aws", "axa", "azure", "baby", "baidu", "banamex", "bananarepublic",
            "band", "bank", "bar", "barcelona", "barclaycard", "barclays", "barefoot", "bargains", "baseball", "basketball",
            "bauhaus", "bayern", "bbc", "bbt", "bbva", "bcg", "bcn", "beats", "beauty", "beer", "bentley", "berlin", "best",
            "bestbuy", "bet", "bharti", "bible", "bid", "bike", "bing", "bingo", "bio", "biz", "black", "blackfriday", "blockbuster",
            "blog", "bloomberg", "blue", "bms", "bmw", "bnpparibas", "boats", "boehringer", "bofa", "bom", "bond", "boo", "book",
            "booking", "bosch", "bostik", "boston", "bot", "boutique", "box", "bradesco", "bridgestone", "broadway", "broker",
            "brother", "brussels", "bugatti", "build", "builders", "business", "buy", "buzz", "bzh", "cab", "cafe", "cal",
            "call", "calvinklein", "cam", "camera", "camp", "cancerresearch", "canon", "capetown", "capital", "capitalone",
            "car", "caravan", "cards", "care", "career", "careers", "cars", "casa", "case", "cash", "casino", "cat", "catering",
            "catholic", "cba", "cbn", "cbre", "cbs", "center", "ceo", "cern", "cfa", "cfd", "chanel", "channel", "charity",
            "chase", "chat", "cheap", "chintai", "christmas", "chrome", "church", "cipriani", "circle", "cisco", "citadel",
            "citi", "citic", "city", "cityeats", "claims", "cleaning", "click", "clinic", "clinique", "clothing", "cloud",
            "club", "clubmed", "coach", "codes", "coffee", "college", "cologne", "com", "comcast", "commbank", "community",
            "company", "compare", "computer", "comsec", "condos", "construction", "consulting", "contact", "contractors", "cooking",
            "cookingchannel", "cool", "coop", "corsica", "country", "coupon", "coupons", "courses", "cpa", "credit", "creditcard",
            "creditunion", "cricket", "crown", "crs", "cruise", "cruises", "cuisinella", "cymru", "cyou", "dabur", "dad", "dance",
            "data", "date", "dating", "datsun", "day", "dclk", "dds", "deal", "dealer", "deals", "degree", "delivery", "dell",
            "deloitte", "delta", "democrat", "dental", "dentist", "desi", "design", "dev", "dhl", "diamonds", "diet", "digital",
            "direct", "directory", "discount", "discover", "dish", "diy", "dnp", "docs", "doctor", "dog", "domains", "dot",
            "download", "drive", "dtv", "dubai", "dunlop", "dupont", "durban", "dvag", "dvr", "earth", "eat", "eco", "edeka",
            "edu", "education", "email", "emerck", "energy", "engineer", "engineering", "enterprises", "epson", "equipment",
            "ericsson", "erni", "esq", "estate", "etisalat", "eurovision", "eus", "events", "exchange", "expert", "exposed",
            "express", "extraspace", "fage", "fail", "fairwinds", "faith", "family", "fan", "fans", "farm", "farmers", "fashion",
            "fast", "fedex", "feedback", "ferrari", "ferrero", "fiat", "fidelity", "fido", "film", "final", "finance", "financial",
            "fire", "firestone", "firmdale", "fish", "fishing", "fit", "fitness", "flickr", "flights", "flir", "florist", "flowers",
            "fly", "foo", "food", "foodnetwork", "football", "ford", "forex", "forsale", "forum", "foundation", "fox", "free",
            "fresenius", "frl", "frogans", "frontdoor", "frontier", "ftr", "fujitsu", "fun", "fund", "furniture", "futbol",
            "fyi", "gal", "gallery", "gallo", "gallup", "game", "games", "gap", "garden", "gay", "gbiz", "gdn", "gea", "gent",
            "genting", "george", "ggee", "gift", "gifts", "gives", "giving", "glass", "gle", "global", "globo", "gmail", "gmbh",
            "gmo", "gmx", "godaddy", "gold", "goldpoint", "golf", "goo", "goodyear", "goog", "google", "gop", "got", "gov",
            "grainger", "graphics", "gratis", "green", "gripe", "grocery", "group", "guardian", "gucci", "guge", "guide", "guitars",
            "guru", "hair", "hamburg", "hangout", "haus", "hbo", "hdfc", "hdfcbank", "health", "healthcare", "help", "helsinki",
            "here", "hermes", "hgtv", "hiphop", "hisamitsu", "hitachi", "hiv", "hkt", "hockey", "holdings", "holiday", "homedepot",
            "homegoods", "homes", "homesense", "honda", "horse", "hospital", "host", "hosting", "hot", "hoteles", "hotels",
            "hotmail", "house", "how", "hsbc", "hughes", "hyatt", "hyundai", "ibm", "icbc", "ice", "icu", "ieee", "ifm", "ikano",
            "imamat", "imdb", "immo", "immobilien", "inc", "industries", "infiniti", "info", "ing", "ink", "institute", "insurance",
            "insure", "int", "international", "intuit", "investments", "ipiranga", "irish", "ismaili", "ist", "istanbul", "itau",
            "itv", "jaguar", "java", "jcb", "jeep", "jetzt", "jewelry", "jio", "jll", "jmp", "jnj", "jobs", "joburg", "jot",
            "joy", "jpmorgan", "jprs", "juegos", "juniper", "kaufen", "kddi", "kerryhotels", "kerrylogistics", "kerryproperties",
            "kfh", "kia", "kim", "kinder", "kindle", "kitchen", "kiwi", "koeln", "komatsu", "kosher", "kpmg", "kpn", "krd",
            "kred", "kuokgroup", "kyoto", "lacaixa", "lamborghini", "lamer", "lancaster", "lancia", "land", "landrover", "lanxess",
            "lasalle", "lat", "latino", "latrobe", "law", "lawyer", "lds", "lease", "leclerc", "lefrak", "legal", "lego", "lexus",
            "lgbt", "lidl", "life", "lifeinsurance", "lifestyle", "lighting", "like", "lilly", "limited", "limo", "lincoln",
            "linde", "link", "lipsy", "live", "living", "llc", "llp", "loan", "loans", "locker", "locus", "loft", "lol", "london",
            "lotte", "lotto", "love", "lpl", "lplfinancial", "ltd", "ltda", "lundbeck", "luxe", "luxury", "macys", "madrid",
            "maif", "maison", "makeup", "man", "management", "mango", "map", "market", "marketing", "markets", "marriott",
            "marshalls", "maserati", "mattel", "mba", "mckinsey", "med", "media", "meet", "melbourne", "meme", "memorial",
            "men", "menu", "merckmsd", "miami", "microsoft", "mil", "mini", "mint", "mit", "mitsubishi", "mlb", "mls", "mma",
            "mobi", "mobile", "moda", "moe", "moi", "mom", "monash", "money", "monster", "mormon", "mortgage", "moscow", "moto",
            "motorcycles", "mov", "movie", "msd", "mtn", "mtr", "museum", "music", "mutual", "nab", "nagoya", "name", "natura",
            "navy", "nba", "nec", "net", "netbank", "netflix", "network", "neustar", "new", "news", "next", "nextdirect", "nexus",
            "nfl", "ngo", "nhk", "nico", "nike", "nikon", "ninja", "nissan", "nissay", "nokia", "northwesternmutual", "norton",
            "now", "nowruz", "nowtv", "nra", "nrw", "ntt", "nyc", "obi", "observer", "office", "okinawa", "olayan", "olayangroup",
            "oldnavy", "ollo", "omega", "one", "ong", "onl", "online", "ooo", "open", "oracle", "orange", "org", "organic",
            "origins", "osaka", "otsuka", "ott", "ovh", "page", "panasonic", "paris", "pars", "partners", "parts", "party",
            "passagens", "pay", "pccw", "pet", "pfizer", "pharmacy", "phd", "philips", "phone", "photo", "photography", "photos",
            "physio", "pics", "pictet", "pictures", "pid", "pin", "ping", "pink", "pioneer", "pizza", "place", "play", "playstation",
            "plumbing", "plus", "pnc", "pohl", "poker", "politie", "porn", "post", "pramerica", "praxi", "press", "prime",
            "pro", "prod", "productions", "prof", "progressive", "promo", "properties", "property", "protection", "pru", "prudential",
            "pub", "pwc", "qpon", "quebec", "quest", "racing", "radio", "read", "realestate", "realtor", "realty", "recipes",
            "red", "redstone", "redumbrella", "rehab", "reise", "reisen", "reit", "reliance", "ren", "rent", "rentals", "repair",
            "report", "republican", "rest", "restaurant", "review", "reviews", "rexroth", "rich", "richardli", "ricoh", "ril",
            "rio", "rip", "rocher", "rocks", "rodeo", "rogers", "room", "rsvp", "rugby", "ruhr", "run", "rwe", "ryukyu", "saarland",
            "safe", "safety", "sakura", "sale", "salon", "samsclub", "samsung", "sandvik", "sandvikcoromant", "sanofi", "sap",
            "sarl", "sas", "save", "saxo", "sbi", "sbs", "sca", "scb", "schaeffler", "schmidt", "scholarships", "school", "schule",
            "schwarz", "science", "scot", "search", "seat", "secure", "security", "seek", "select", "sener", "services", "ses",
            "seven", "sew", "sex", "sexy", "sfr", "shangrila", "sharp", "shaw", "shell", "shia", "shiksha", "shoes", "shop",
            "shopping", "shouji", "show", "showtime", "silk", "sina", "singles", "site", "ski", "skin", "sky", "skype", "sling",
            "smart", "smile", "sncf", "soccer", "social", "softbank", "software", "sohu", "solar", "solutions", "song", "sony",
            "soy", "spa", "space", "sport", "spot", "srl", "stada", "staples", "star", "statebank", "statefarm", "stc", "stcgroup",
            "stockholm", "storage", "store", "stream", "studio", "study", "style", "sucks", "supplies", "supply", "support",
            "surf", "surgery", "suzuki", "swatch", "swiss", "sydney", "systems", "tab", "taipei", "talk", "taobao", "target",
            "tatamotors", "tatar", "tattoo", "tax", "taxi", "tci", "tdk", "team", "tech", "technology", "tel", "temasek", "tennis",
            "teva", "thd", "theater", "theatre", "tiaa", "tickets", "tienda", "tiffany", "tips", "tires", "tirol", "tjmaxx",
            "tjx", "tkmaxx", "tmall", "today", "tokyo", "tools", "top", "toray", "toshiba", "total", "tours", "town", "toyota",
            "toys", "trade", "trading", "training", "travel", "travelchannel", "travelers", "travelersinsurance", "trust",
            "trv", "tube", "tui", "tunes", "tushu", "tvs", "ubank", "ubs", "unicom", "university", "uno", "uol", "ups", "vacations",
            "vana", "vanguard", "vegas", "ventures", "verisign", "versicherung", "vet", "viajes", "video", "vig", "viking",
            "villas", "vin", "vip", "virgin", "visa", "vision", "viva", "vivo", "vlaanderen", "vodka", "volkswagen", "volvo",
            "vote", "voting", "voto", "voyage", "vuelos", "wales", "walmart", "walter", "wang", "wanggou", "watch", "watches",
            "weather", "weatherchannel", "webcam", "weber", "website", "wed", "wedding", "weibo", "weir", "whoswho", "wien",
            "wiki", "williamhill", "win", "windows", "wine", "winners", "wme", "wolterskluwer", "woodside", "work", "works",
            "world", "wow", "wtc", "wtf", "xbox", "xerox", "xfinity", "xihuan", "xin", "xn--11b4c3d", "कॉम", "xn--1ck2e1b", "セール",
            "xn--1qqw23a", "佛山", "xn--2scrj9c", "ಭಾರತ", "xn--30rr7y", "慈善", "xn--3bst00m", "集团", "xn--3ds443g", "在线", "xn--3e0b707e", "한국",
            "xn--3hcrj9c", "ଭାରତ", "xn--3pxu8k", "点看", "xn--42c2d9a", "คอม", "xn--45br5cyl", "ভাৰত", "xn--45brj9c", "ভারত",
            "xn--45q11c", "八卦", "xn--4dbrk0ce", "ישראל", "xn--4gbrim", "موقع", "xn--54b7fta0cc", "বাংলা", "xn--55qw42g", "公益",
            "xn--55qx5d", "公司", "xn--5su34j936bgsg", "香格里拉", "xn--5tzm5g", "网站", "xn--6frz82g", "移动", "xn--6qq986b3xl", "我爱你",
            "xn--80adxhks", "москва", "xn--80ao21a", "қаз", "xn--80aqecdr1a", "католик", "xn--80asehdb", "онлайн", "xn--80aswg", "сайт",
            "xn--8y0a063a", "联通", "xn--90a3ac", "срб", "xn--90ae", "бг", "xn--90ais", "бел", "xn--9dbq2a", "קום", "xn--9et52u", "时尚",
            "xn--9krt00a", "微博", "xn--b4w605ferd", "淡马锡", "xn--bck1b9a5dre4c", "ファッション", "xn--c1avg", "орг", "xn--c2br7g", "नेट",
            "xn--cck2b3b", "ストア", "xn--cckwcxetd", "アマゾン", "xn--cg4bki", "삼성", "xn--clchc0ea0b2g2a9gcd", "சிங்கப்பூர்", "xn--czr694b", "商标",
            "xn--czrs0t", "商店", "xn--czru2d", "商城", "xn--d1acj3b", "дети", "xn--d1alf", "мкд", "xn--e1a4c", "ею", "xn--eckvdtc9d", "ポイント",
            "xn--efvy88h", "新闻", "xn--fct429k", "家電", "xn--fhbei", "كوم", "xn--fiq228c5hs", "中文网", "xn--fiq64b", "中信", "xn--fiqs8s", "中国",
            "xn--fiqz9s", "中國", "xn--fjq720a", "娱乐", "xn--flw351e", "谷歌", "xn--fpcrj9c3d", "భారత్", "xn--fzc2c9e2c", "ලංකා",
            "xn--fzys8d69uvgm", "電訊盈科", "xn--g2xx48c", "购物", "xn--gckr3f0f", "クラウド", "xn--gecrj9c", "ભારત", "xn--gk3at1e", "通販",
            "xn--h2breg3eve", "भारतम्", "xn--h2brj9c", "भारत", "xn--h2brj9c8c", "भारोत", "xn--hxt814e", "网店", "xn--i1b6b1a6a2e", "संगठन",
            "xn--imr513n", "餐厅", "xn--io0a7i", "网络", "xn--j1aef", "ком", "xn--j1amh", "укр", "xn--j6w193g", "香港", "xn--jlq480n2rg", "亚马逊",
            "xn--jlq61u9w7b", "诺基亚", "xn--jvr189m", "食品", "xn--kcrx77d1x4a", "飞利浦", "xn--kprw13d", "台湾", "xn--kpry57d", "台灣",
            "xn--kput3i", "手机", "xn--l1acc", "мон", "xn--lgbbat1ad8j", "الجزائر", "xn--mgb9awbf", "عمان", "xn--mgba3a3ejt", "ارامكو",
            "xn--mgba3a4f16a", "ایران", "xn--mgba7c0bbn0a", "العليان", "xn--mgbaakc7dvf", "اتصالات", "xn--mgbaam7a8h", "امارات",
            "xn--mgbab2bd", "بازار", "xn--mgbah1a3hjkrd", "موريتانيا", "xn--mgbai9azgqp6j", "پاکستان", "xn--mgbayh7gpa", "الاردن",
            "xn--mgbbh1a", "بارت", "xn--mgbbh1a71e", "بھارت", "xn--mgbc0a9azcg", "المغرب", "xn--mgbca7dzdo", "ابوظبي", "xn--mgbcpq6gpa1a", "البحرين",
            "xn--mgberp4a5d4ar", "السعودية", "xn--mgbgu82a", "ڀارت", "xn--mgbi4ecexp", "كاثوليك", "xn--mgbpl2fh", "سودان",
            "xn--mgbt3dhd", "همراه", "xn--mgbtx2b", "عراق", "xn--mgbx4cd0ab", "مليسيا", "xn--mix891f", "澳門", "xn--mk1bu44c", "닷컴",
            "xn--mxtq1m", "政府", "xn--ngbc5azd", "شبكة", "xn--ngbe9e0a", "بيتك", "xn--ngbrx", "عرب", "xn--node", "გე", "xn--nqv7f", "机构",
            "xn--nqv7fs00ema", "组织机构", "xn--nyqy26a", "健康", "xn--o3cw4h", "ไทย", "xn--ogbpf8fl", "سورية", "xn--otu796d", "招聘",
            "xn--p1acf", "рус", "xn--p1ai", "рф", "xn--pgbs0dh", "تونس", "xn--pssy2u", "大拿", "xn--q7ce6a", "ລາວ", "xn--q9jyb4c", "みんな",
            "xn--qcka1pmc", "グーグル", "xn--qxa6a", "ευ", "xn--qxam", "ελ", "xn--rhqv96g", "世界", "xn--rovu88b", "書籍", "xn--rvc1e0am3e", "ഭാരതം",
            "xn--s9brj9c", "ਭਾਰਤ", "xn--ses554g", "网址", "xn--t60b56a", "닷넷", "xn--tckwe", "コム", "xn--tiq49xqyj", "天主教", "xn--unup4y", "游戏",
            "xn--vermgensberater-ctb", "vermögensberater", "xn--vermgensberatung-pwb", "vermögensberatung", "xn--vhquv", "企业",
            "xn--vuq861b", "信息", "xn--w4r85el8fhu5dnra", "嘉里大酒店", "xn--w4rs40l", "嘉里", "xn--wgbh1c", "مصر", "xn--wgbl6a", "قطر",
            "xn--xhq521b", "广东", "xn--xkc2al3hye2a", "இலங்கை", "xn--xkc2dl3a5ee0h", "இந்தியா", "xn--y9a3aq", "հայ", "xn--yfro4i67o", "新加坡",
            "xn--ygbi2ammx", "فلسطين", "xn--zfr164b", "政务", "xxx", "xyz", "yachts", "yahoo", "yamaxun", "yandex", "yodobashi",
            "yoga", "yokohama", "you", "youtube", "yun", "zappos", "zara", "zero", "zip", "zone", "zuerich"
        };

        var ctlds = new[]
        {
            "ac", // Ascension Island
				"ad", // Andorra
				"ae", // United Arab Emirates
				"af", // Afghanistan
				"ag", // Antigua and Barbuda
				"ai", // Anguilla
				"al", // Albania
				"am", // Armenia
				"an", // Netherlands Antilles
				"ao", // Angola
				"aq", // Antarctica
				"ar", // Argentina
				"as", // American Samoa
				"at", // Austria
				"au", // Australia
				"aw", // Aruba
				"ax", // Aland Islands
				"az", // Azerbaijan
				"ba", // Bosnia and Herzegovina
				"bb", // Barbados
				"bd", // Bangladesh
				"be", // Belgium
				"bf", // Burkina Faso
				"bg", // Bulgaria
				"bh", // Bahrain
				"bi", // Burundi
				"bj", // Benin
				"bm", // Bermuda
				"bn", // Brunei Darussalam
				"bo", // Bolivia
				"br", // Brazil
				"bs", // Bahamas
				"bt", // Bhutan
				"bv", // Bouvet Island
				"bw", // Botswana
				"by", // Belarus
				"bz", // Belize
				"ca", // Canada
				"cc", // Cocos (Keeling) Islands
				"cd", // Congo, The Democratic Republic of the
				"cf", // Central African Republic
				"cg", // Congo, Republic of
				"ch", // Switzerland
				"ci", // Cote d'Ivoire
				"ck", // Cook Islands
				"cl", // Chile
				"cm", // Cameroon
				"cn", // China
				"co", // Colombia
				"cr", // Costa Rica
				"cs", // Serbia and Montenegro
				"cu", // Cuba
				"cv", // Cape Verde
				"cx", // Christmas Island
				"cy", // Cyprus
				"cz", // Czech Republic
				"de", // Germany
				"dj", // Djibouti
				"dk", // Denmark
				"dm", // Dominica
				"do", // Dominican Republic
				"dz", // Algeria
				"ec", // Ecuador
				"ee", // Estonia
				"eg", // Egypt
				"eh", // Western Sahara
				"er", // Eritrea
				"es", // Spain
				"et", // Ethiopia
				"eu", // European Union
				"fi", // Finland
				"fj", // Fiji
				"fk", // Falkland Islands (Malvinas)
				"fm", // Micronesia, Federal State of
				"fo", // Faroe Islands
				"fr", // France
				"ga", // Gabon
				"gb", // United Kingdom
				"gd", // Grenada
				"ge", // Georgia
				"gf", // French Guiana
				"gg", // Guernsey
				"gh", // Ghana
				"gi", // Gibraltar
				"gl", // Greenland
				"gm", // Gambia
				"gn", // Guinea
				"gp", // Guadeloupe
				"gq", // Equatorial Guinea
				"gr", // Greece
				"gs", // South Georgia and the South Sandwich Islands
				"gt", // Guatemala
				"gu", // Guam
				"gw", // Guinea-Bissau
				"gy", // Guyana
				"hk", // Hong Kong
				"hm", // Heard and McDonald Islands
				"hn", // Honduras
				"hr", // Croatia/Hrvatska
				"ht", // Haiti
				"hu", // Hungary
				"id", // Indonesia
				"ie", // Ireland
				"il", // Israel
				"im", // Isle of Man
				"in", // India
				"io", // British Indian Ocean Territory
				"iq", // Iraq
				"ir", // Iran, Islamic Republic of
				"is", // Iceland
				"it", // Italy
				"je", // Jersey
				"jm", // Jamaica
				"jo", // Jordan
				"jp", // Japan
				"ke", // Kenya
				"kg", // Kyrgyzstan
				"kh", // Cambodia
				"ki", // Kiribati
				"km", // Comoros
				"kn", // Saint Kitts and Nevis
				"kp", // Korea, Democratic People's Republic
				"kr", // Korea, Republic of
				"kw", // Kuwait
				"ky", // Cayman Islands
				"kz", // Kazakhstan
				"la", // Lao People's Democratic Republic
				"lb", // Lebanon
				"lc", // Saint Lucia
				"li", // Liechtenstein
				"lk", // Sri Lanka
				"lr", // Liberia
				"ls", // Lesotho
				"lt", // Lithuania
				"lu", // Luxembourg
				"lv", // Latvia
				"ly", // Libyan Arab Jamahiriya
				"ma", // Morocco
				"mc", // Monaco
				"md", // Moldova, Republic of
				"mg", // Madagascar
				"mh", // Marshall Islands
				"mk", // Macedonia, The Former Yugoslav Republic of
				"ml", // Mali
				"mm", // Myanmar
				"mn", // Mongolia
				"mo", // Macau
				"mp", // Northern Mariana Islands
				"mq", // Martinique
				"mr", // Mauritania
				"ms", // Montserrat
				"mt", // Malta
				"mu", // Mauritius
				"mv", // Maldives
				"mw", // Malawi
				"mx", // Mexico
				"my", // Malaysia
				"mz", // Mozambique
				"na", // Namibia
				"nc", // New Caledonia
				"ne", // Niger
				"nf", // Norfolk Island
				"ng", // Nigeria
				"ni", // Nicaragua
				"nl", // Netherlands
				"no", // Norway
				"np", // Nepal
				"nr", // Nauru
				"nu", // Niue
				"nz", // New Zealand
				"om", // Oman
				"pa", // Panama
				"pe", // Peru
				"pf", // French Polynesia
				"pg", // Papua New Guinea
				"ph", // Philippines
				"pk", // Pakistan
				"pl", // Poland
				"pm", // Saint Pierre and Miquelon
				"pn", // Pitcairn Island
				"pr", // Puerto Rico
				"ps", // Palestinian Territory, Occupied
				"pt", // Portugal
				"pw", // Palau
				"py", // Paraguay
				"qa", // Qatar
				"re", // Reunion Island
				"ro", // Romania
				"rs", // Serbia
				"ru", // Russian Federation
				"rw", // Rwanda
				"sa", // Saudi Arabia
				"sb", // Solomon Islands
				"sc", // Seychelles
				"sd", // Sudan
				"se", // Sweden
				"sg", // Singapore
				"sh", // Saint Helena
				"si", // Slovenia
				"sj", // Svalbard and Jan Mayen Islands
				"sk", // Slovak Republic
				"sl", // Sierra Leone
				"sm", // San Marino
				"sn", // Senegal
				"so", // Somalia
				"sr", // Suriname
				"st", // Sao Tome and Principe
				"sv", // El Salvador
				"sy", // Syrian Arab Republic
				"sz", // Swaziland
				"tc", // Turks and Caicos Islands
				"td", // Chad
				"tf", // French Southern Territories
				"tg", // Togo
				"th", // Thailand
				"tj", // Tajikistan
				"tk", // Tokelau
				"tl", // Timor-Leste
				"tm", // Turkmenistan
				"tn", // Tunisia
				"to", // Tonga
				"tp", // East Timor
				"tr", // Turkey
				"tt", // Trinidad and Tobago
				"tv", // Tuvalu
				"tw", // Taiwan
				"tz", // Tanzania
				"ua", // Ukraine
				"ug", // Uganda
				"uk", // United Kingdom
				"um", // United States Minor Outlying Islands
				"us", // United States
				"uy", // Uruguay
				"uz", // Uzbekistan
				"va", // Holy See (Vatican City State)
				"vc", // Saint Vincent and the Grenadines
				"ve", // Venezuela
				"vg", // Virgin Islands, British
				"vi", // Virgin Islands, U.S.
				"vn", // Vietnam
				"vu", // Vanuatu
				"wf", // Wallis and Futuna Islands
				"ws", // Western Samoa
				"ye", // Yemen
				"yt", // Mayotte
				"yu", // Yugoslavia
				"za", // South Africa
				"zm", // Zambia
				"zw"  // Zimbabwe
			};

        _tlds = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var tld in tlds)
            _tlds.Add(tld);
        foreach (var ctld in ctlds)
            _tlds.Add(ctld);
    }
    public static bool IsValidEmail([NotNullWhen(true)] string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Length < 6 || email.Length > 128)
            return false;

        int pos = email.IndexOf('@');
        if (pos <= 0)
            return false;

        string alias = email[..pos];
        if (string.IsNullOrWhiteSpace(alias) || alias.Length > 64)
            return false;

        if (alias[0] == '\"')
        {
            // Quoted email alias
            if (alias[alias.Length - 1] != '\"')
                return false;

            alias = alias[1..^1];
        }

        // must start with letter or digit
        if (!char.IsLetterOrDigit(alias, 0))
            return false;

        // must end in letter or digit or underscore or dash
        char last = alias[^1];
        if (!last.IsASCIILetterOrDigit() && last != '_' && last != '-')
            return false;

        foreach (char c in alias)
        {
            if (c >= 'a' && c <= 'z')
                continue;
            if (c >= 'A' && c <= 'Z')
                continue;
            if (c >= '0' && c <= '9')
                continue;
            if ("-_%.=/+'".IndexOf(c) >= 0)
                continue;
            return false;
        }

        string domain = email.Substring(pos + 1);
        return IsValidDomain(domain, true);
    }


    public static bool IsValidLink([NotNullWhen(true)] string? link, bool acceptRelative = false)
    {
        if (link == null)
            return false;
        try
        {
            var uri = new Uri(link);
            if (!uri.IsAbsoluteUri)
                return acceptRelative;

            switch (uri.Scheme.ToLowerInvariant())
            {
                case "http":
                case "https":
                    if (!IsValidDomain(uri.Host))
                        return false;
                    break;
                case "mailto":
                    if (!IsValidEmail(uri.UserInfo + "@" + uri.Host))
                        return false;
                    break;
                case "file":
                case "ftp":
                case "gopher":
                case "ldap":
                case "net.pipe":
                case "net.tcp":
                case "news":
                case "nntp":
                case "telnet":
                case "uuid":
                    break;
                default:
                    return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool IsValidDomain([NotNullWhen(true)] string? domainName, bool internetValid = true)
    {
        if (domainName == null || domainName.Length < 4)
            return false;

        if (IsValidIP(domainName))
            return true;

        var parts = domainName.Split('.');
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                return false;

            var p2 = part.ToLower();
            if (p2[0] == '-' || p2[^1] == '-')
                return false; // cannot start or end with a dash

            foreach (char c in p2)
            {
                if (c >= 'a' && c <= 'z')
                    continue;
                if (c >= '0' && c <= '9')
                    continue;
                if (c == '-')
                    continue;
                return false;
            }
        }

        if (internetValid)
        {
            if (domainName.Equals("locahost", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (parts.Length < 2)
                return false; // Must have at least a host and TLD
            string tld = parts[^1];
            return _tlds.Contains(tld);
        }
        return true;
    }

    public static bool IsValidUSPhoneNumber([NotNullWhen(true)] string? phoneNumber, bool withAreaCode = true)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        if (phoneNumber.Length < 7)
            return false;

        var allDigits = new StringBuilder();

        int p1 = phoneNumber.IndexOf('+');
        if (p1 >= 0)
        {
            if (p1 == phoneNumber.Length - 1)
                return false;

            string nextCountryDigit = phoneNumber.Substring(p1 + 1, 1);
            if (nextCountryDigit != "1")
                return false;

            phoneNumber = phoneNumber.Substring(p1 + 2);
        }

        foreach (char c in phoneNumber)
        {
            if (Char.IsDigit(c))
                allDigits.Append(c);
            else if (_validPhoneNumberSymbols.IndexOf(c) == -1)
                return false;
        }

        string digits = allDigits.ToString();

        if (digits.Length == 10)
        {
            string areaCode = digits.Substring(0, 3);
            if (!_usAreaCodes.ContainsKey(areaCode))
                return false;
            digits = digits.Substring(3);
        }
        else if (withAreaCode)
            return false;

        if (digits.Length != 7)
            return false;

        // cannot start with 0 or 1
        if (digits[0] == '0' || digits[0] == '1')
            return false;

        // cannot start with 555
        if (digits.StartsWith("555"))
            return false;

        return true;
    }

    public static bool IsValidPhoneNumber([NotNullWhen(true)] string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        if (phoneNumber.Length < 6)
            return false;

        int digitCount = 0;
        foreach (char c in phoneNumber)
        {
            if (char.IsDigit(c))
            {
                digitCount++;
                continue;
            }
            else if (char.IsControl(c) || char.IsWhiteSpace(c))
            {
                continue;
            }
            if (!_validPhoneNumberSymbols.Contains(c))
            {
                return false;
            }
        }

        return digitCount >= 6 && digitCount <= 14;
    }

    public static bool IsValidBase64String(string? base64String)
    {
        if (string.IsNullOrEmpty(base64String))
            return true;

        foreach (char c in base64String)
        {
            if (c >= 'a' && c <= 'z')
                continue;
            if (c >= 'A' && c <= 'Z')
                continue;
            if (c >= '0' && c <= '9')
                continue;
            if (c == '=' || c == '+' || c == '/')
                continue;
            return false;
        }

        return true;
    }

    public static bool IsValidHtmlColor([NotNullWhen(true)] string? htmlColor)
    {
        var color = ColorExtensions.ToColor(htmlColor);
        return color != null;
    }

    public static bool IsValidTwitterUsername([NotNullWhen(true)] string? twitterUsername)
    {
        if (string.IsNullOrWhiteSpace(twitterUsername))
            return false;

        int start = 0;
        if (twitterUsername.StartsWith("@"))
            start = 1;

        if (twitterUsername.Length - start > 15)
            return false; // max 15 characters

        for (int i = start; i < twitterUsername.Length; i++)
        {
            char c = twitterUsername[i];
            if (c >= 'a' && c <= 'z')
                continue;
            if (c >= 'A' && c <= 'Z')
                continue;
            if (c >= '0' && c <= '9')
                continue;
            if (c == '_')
                continue;
            return false;
        }

        if (start == 1)
            twitterUsername = twitterUsername.Substring(1);

        return !_notValidTwitterAccounts.Contains(twitterUsername);
    }

    /// <summary>
    /// Validates a the "to" part of the mailto: URI
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidMailToAddress([NotNullWhen(true)] string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        int ltp = email.IndexOf('<');
        if (ltp >= 0)
        {
            // Composite email as in "Name <email>"
            if (ltp > 0)
            {
                string name = email[..ltp].Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    foreach (char c in name)
                    {
                        if (char.IsControl(c))
                            return false;
                    }
                }
            }

            int gtp = email.IndexOf('>');
            if (gtp != email.Length - 1)
                return false; // must be the last character

            email = email.Substring(ltp + 1, email.Length - ltp - 2).Trim();
            if (email == null || email.Length < 6)
                return false;
        }

        return IsValidEmail(email);
    }

    public static bool IsValidDomainTLD([NotNullWhen(true)] string? tld)
    {
        if (string.IsNullOrWhiteSpace(tld) || tld.Length < 2)
            return false;

        if (tld[0] == '.')
        {
            tld = tld[1..];
            if (tld.Length < 2)
                return false;
        }

        return _tlds.Contains(tld);
    }

    /// <summary>
    /// Validates if the string contains a GUID
    /// </summary>
    public static bool IsValidGuid([NotNullWhen(true)] string? guid)
    {
        if (string.IsNullOrEmpty(guid))
            return false;

        try
        {
            Guid.Parse(guid);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates if the string contains a well-formed dot-quad IP address v4.
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public static bool IsValidIPv4([NotNullWhen(true)] string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        // We are a little more restrictive than IPAddress
        int dotCount = ipAddress.Count(c => c == '.');
        if (dotCount != 3)
            return false;

        if (!IPAddress.TryParse(ipAddress, out var ip))
            return false;

        return ip.AddressFamily == AddressFamily.InterNetwork;
    }

    /// <summary>
    /// Validates if the string contains a well-formed IP address v6
    /// </summary>
    public static bool IsValidIPv6([NotNullWhen(true)] string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        if (!IPAddress.TryParse(ipAddress, out var ip))
            return false;

        return ip.AddressFamily == AddressFamily.InterNetworkV6;
    }

    /// <summary>
    /// Validates if the string contains an IP Address v4 or v6
    /// </summary>
    public static bool IsValidIP([NotNullWhen(true)] string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        if (!IPAddress.TryParse(ipAddress, out var ip))
            return false;

        return ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6;
    }

    /// <summary>
    /// Validates a time string
    /// </summary>
    public static bool IsValidTime([NotNullWhen(true)] string? timeString)
    {
        var time = DateTimeExtensions.ParseTime(timeString);
        return time != null;
    }

    /// <summary>
    /// Validates a date string (using Gregorian calendar)
    /// </summary>
    public static bool IsValidDate([NotNullWhen(true)] string? dateString,
        DatePartsOrder datePartsOrder = DatePartsOrder.MDY)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        if (dateString.Length < 5)
            return false;

        var parts = dateString.Split(new char[] { '.', '/', '-' });
        if (parts.Length != 3)
            return false;

        static int parsePart(string s)
        {
            s = s.Trim();
            if (s.Length == 0)
                return -1;

            if (!int.TryParse(s, out var ret))
                return -1;
            return ret;
        }

        int part1 = parsePart(parts[0]);
        if (part1 == -1)
            return false;

        int part2 = parsePart(parts[1]);
        if (part2 == -1)
            return false;

        int part3 = parsePart(parts[2]);
        if (part3 == -1)
            return false;

        int m, d, y;
        switch (datePartsOrder)
        {
            case DatePartsOrder.MDY:
                m = part1;
                d = part2;
                y = part3;
                break;
            case DatePartsOrder.YMD:
                y = part1;
                m = part2;
                d = part3;
                break;
            case DatePartsOrder.DMY:
                d = part1;
                m = part2;
                y = part3;
                break;
            default:
                throw new NotImplementedException("Unsupported DatePartsOrder.");
        }

        // Up for debate, but in practical terms, a year so much in the future is probably not
        // the intent of validating a date in a typical user input scenario.
        if (y < 1 || y > 2200 || m < 1 || m > 12 || d < 1)
            return false;
        if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
        {
            if (d > 31)
                return false;
        }
        else if (m == 2)
        {
            if (DateTime.IsLeapYear(y))
            {
                if (d > 29)
                    return false;
            }
            else if (d > 28)
                return false;
        }
        else if (d > 30)
        {
            return false;
        }

        return true;
    }


}
