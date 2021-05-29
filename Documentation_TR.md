# Blazor Boilerplate Dokümantasyon

Bu döküman Client tarafında Blazor WebAssembly, Server tarafında ise .NET Core kullanılan projenin nasıl geliştirildiğini anlatmak için hazırlanmıştır.

### Gereksinimler
* .NET Core
* Visual Studio'nun güncel versiyonu

# Projelerin Oluşturulması

*	Visual Studio'da ilk önce ASP.NET Core Web App oluşturulur.
	*	Burada dikkat edilmesi gerekenler şunlardır:
		* Eğer mobil proje eklenecek ise Target Runtime .NET Core 3.1 seçilmelidir.
		* Authentication seçeneğinden Individual Accounts Seçilmelidir.
		* Configure for HTTPS seçili olmalıdır
	
	* Bu Proje bize içerik sağlayacak API'ye erişmemizi sağlayacak token üretecek ve dolayısı ile Authentication oluşturacaktır. Sadece burada ürettiğimiz token'ı alıp API'mize istekte bulunabileceğiz.

*	Sonraki adım olarak diğer projeler eklenir:
	* API projesi : ASP.NET Core Web API projesi, Target Framework: .NET Core 3.1
		*Bu proje oluşturacağımız tüm içeriği bize getirmekle görevli olacak.
	
	* Admin UI : Blazor WebAssembly projesi. Target Framework: .NET Core 3.1
		* Bu proje Admin Paneli olarak iş görecektir.
		* Admin paneli içerisinden database'e kayıt eklenebilir, kullanıcı işlemleri yapılabilir.
		* Admin panelinde kullanıcı girişi gereklidir.
	
	* Client UI : Blazor WebAssembly projesi. Target Framework: .NET Core 3.1
		* Normal kullanıcıların göreceği Blazor WebAssembly uygulaması.
	
	* Shared.Models : .NET Standard Class Library projesi, Target Framework: .NET Standard 2.1
		* Bu projede kullanacağımız tüm ortak classlar bulunmaktadır

	* Shared.Services : .NET Standard Class Library projesi, Target Framework: .NET Standard 2.1
		* Bu projede iletişimi sağlayacak tüm servisler bulunmaktadır.


* Projeler eklendikten sonra ```Boilerplate.AuthServer``` projesinden ```Pages```, ```Shared``` ve ```wwwroot``` klasörleri silinir.
Buradaki amaç bizim bu projede bir sayfa görüntülemeyecek olmamızdır.

* ```Boilerplate.AuthServer``` projesinde ```appsettings.json``` dosyası açılır ve connectionString kendi Database Server Engine'nınıza göre değiştirilir.
	* Sonrasında ise ya Package Manager Console üzerinden ya da Developer PowerShell açılır.
		* Eğer Package Manager Console açıldıysa Default Project Boilerplate.AuthServer seçilerek ```update-database``` komutu girilir.
		* Eğer Developer Powershell açıldıysa AuthServer projesinin bulunduğu klasöre girilir ve ```dotnet ef database update``` komutu girilir. (Ef paketini indirmeniz gerekebilir)
	* Bu işlem Identity için gerekli tabloları database'de otomatik olarak oluşturacaktır.
	* Bu proje'de sadece bu adımda ```Code-First``` mantığı ile işlem yapılmaktadır. Identity ile çalışılmak isteniyorsa bunun alternatifi Identity Server'ın oluşturduğu tabloları tek tek Database'imizde oluşturmak olacaktır.
	* Bu projeye benzer bir geliştirme yapmak isteyip Identity kullanmak istemeyen kişiler bu adımı es geçebilir.

* ```API``` ve ```AuthServer``` projelerine hem ```Shared.Models``` hem de ```Shared.Services``` projelerine referans verilir.
	* Buradaki amaç tüm işlem yapacak projelerin birbirinden bağımsız olmasına rağmen ortak kullandığı class'lara erişebilmesini sağlamaktır.

* Projelere referans verildikten sonra artık tablolarımızı ortak projelerde koda dönüştürmeye başlayabiliriz.
	* Başlamadan önce bazı projelere NuGet paketleri yüklememiz gerekmektedir:
		
		* AuthServer Projesi:
			* Microsoft.EntityFrameworkCore" Version="3.1.15"
			* "Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="3.1.5"
		
		* Shared.Models Projesi:
			* "Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="3.1.15"
			* "Microsoft.EntityFrameworkCore" Version="3.1.15"
			* "Microsoft.EntityFrameworkCore.SqlServer" Version="3.1.15"
			* "Microsoft.EntityFrameworkCore.Tools" Version="3.1.15"

	* NuGet paketleri kurulurken dikkat etmemiz gereken bazı durumlar vardır:
		* Elimizden geldiğince Solution'da bulunan tüm paketlerin aynı versiyonda olmasına özen göstermeliyiz. Örnek olarak X paketinin 1.1.1 versiyonunu kullanan AuthServer projesi ile aynı paketin 1.1.2 versiyonunu kullanan diğer bir proje işlemlerimizde problem çıkabilmektedir. Patch'lerde sıkıntı olmasa da hem minör hem de major versiyon değişikliklerinde problemler oluşması olasıdır.
		* Paketleri incelediğimizde birbirine bağlı birçok paket bulunmaktadır. Bundan dolayı yapacağımız bir paket upgrade'i -ya da downgrade'i- bağımlı olduğu paketten dolayı başarılı olmayabilir. Öncesinde temel paketi downgrade/upgrade edip sonra diğer paketlere sırayla aynı işlemi uygulamak gerekmektedir.
		* .NET 5 versiyonunu bu projede kullanmadım çünkü mobil projesi olacak Xamarin'in .NET 5 runtime'ına desteği yok. .NET Standard 2.1 Runtime'ı Xamarin'i desteklemekte. Bu yüzden .NET Standard 2.1 ile Class Library'lerimizi oluşturduk. .NET Core 3.1 projelerinden de .NET Standard 2.1'e referans verebilmekteyiz.
		* Sadece Web olacak projelerde tüm projeler .NET 5 ile kurulabilir.

	* Paketleri kurduktan sonra oluşan tablolarımızın classlarını oluşturmak için ```Scaffold``` işlemi yapmamız gerekmektedir.
		* Bunun için Package Manager Console'dan önce Default Project olarak ```Shared.Models``` projesi seçilir.
		* Startup Project olarak da ```Boilerplate.AuthServer``` projesi seçilmelidir.
		* Sonrasında ise ```Scaffold-DbContext "***ConnectionStringiniz***" Microsoft.EntityFrameworkCore.SqlServer -OutputDir ***KlasörAdı***``` komutu girilir.
			* Bu adımın çalışması için solution içerisinde syntax hatası gibi errorlar bulunmamalıdır ve tüm solution'ınız başarıyla derlenebilmelidir.
			* Burada oluşabilecek yaygın hatalardan biri ConnectionString kısmındadır. Klasörleri ayırt etmek için tek \ (ters slash) yeterlidir. appsetting.json içerisinden direkt kopyalarsanız iki adet ters slash olduğundan komut çalışmayacaktır.
		* Oluşan classları kullanabilmemiz için oluşan dbContextclassını AuthServer projesinin Startup.cs'inde ```services.AddDbContext<***DbAdınız***Context>();``` şeklinde tanımlamamız gereklidir.

	* Bu aşamaya kadar ana projelerimizi açtık. Şimdi AuthServer Projesine JWT ekleyelim.
		* Öncelikle kurmamız gereken bir NuGEt paketi var: ```"Microsoft.AspNetCore.Authentication.JwtBearer" Version="3.1.15"```
		* [Bu committe](https://github.com/ktaze1/BlazorBoilerplate/commit/b626fedfbc93500cd2993f0d5f2f9c350e9ce4a0#diff-fd1ae142d344ffd7e125fc721f6e60494d712181fc2f69450f62cb2939966ff0) görüleceği üzere 35. satırdan itibaren Authentication için gerekli configürasyonları yaptık. İçine verdiğimiz değerler token oluşturulurken bu tokenin bu proje tarafından oluşturulduğunu ve kimlerin bu token'ı kullanbileceğini göstermektedir.
		* Blazor projelerimizden API ve Auth Server'a ulaşmak için ise Cross-Origin Resource Sharing'e (CORS) izin vermemiz gerekmektedir. 
		* ConfigureServices metodunun içinde tanımlamalarımız yaptıktan sonra Configure metodu içerisinde ```UseCors``` ve ```useAuthentication``` metodlarını çağırmamız gerekmektedir.
			* Burada dikkat edilmesi gereken nokta ```app.UseAuthentication``` metodunun ```app.UseAuthorization```dan önce kullanılması gerektiğidir. Bu kısımda komutlar sırayla çalıştığından tersi durumda proje çalışmayacaktır. 
		* Tokenımızı temsil edecek [class'ı](https://github.com/ktaze1/BlazorBoilerplate/commit/b626fedfbc93500cd2993f0d5f2f9c350e9ce4a0#diff-d19ed6a05f582589c6fc1310669da371de7413cf8cc895f93fd42a58064070f0) tanımlayalım.
		* Sonrasında ise bize token üretecek [class interface ile beraber](https://github.com/ktaze1/BlazorBoilerplate/commit/b626fedfbc93500cd2993f0d5f2f9c350e9ce4a0#diff-aeaa92ec45d804964e26ff6654096b9887745d28890f4212d3e095a934cd20aa) eklenmiştir.
		* Token üretimi proje çalıştığı sürece çalışması gerektiğinden bunu Singleton olarak Startup dosyasında [ekliyoruz](https://github.com/ktaze1/BlazorBoilerplate/commit/b626fedfbc93500cd2993f0d5f2f9c350e9ce4a0#diff-963b7276df42fb0d6f348b0a7553e3823485734a7652cf8221e35a54ba14de77R88).
		* Hem CORS hem de JWT ayarları appsetting.json'a [ekliyoruz](https://github.com/ktaze1/BlazorBoilerplate/commit/b626fedfbc93500cd2993f0d5f2f9c350e9ce4a0#diff-26beac0f6db315702d8d1d6458f730585efdaa1909a218143e367585c9a27678).

	* Blazor tarafında Authentication'ı sağlayacak kısım şu şekilde hazırlanmıştır. İlk olarak AdminUI projesinde uygulanmıştır fakat aynı adımlar ClientUI projesinde de isteğe göre uygulanabilir.
		
		* Öncelikle bazı NuGet paketleri kurmamız gerekmektedir:
			* "Blazored.LocalStorage" Version="4.1.1" (Beni Hatırla gibi özellikler için.)
			* "Blazored.SessionStorage" Version="2.1.0"
			* "Microsoft.AspNetCore.Components.Authorization" Version="3.1.15"
			* "Microsoft.AspNetCore.Http.Abstractions" Version="2.2.0"
			* "Microsoft.Extensions.Http" Version="3.1.15"
			* "System.Net.Http.Json" Version="3.2.1"
		
		* Paketleri kurduktan sonra ```App.razor```dosyasında [değişiklik yapmalıyız](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7).
			* Burada ilk olarak oluşturulmuş tüm tagler ```CascadingAutheticationState``` içine alınmıştır. Bu AuthenticationState'i ne kadar derinde olsun tüm componentlerde kullabilmemizi sağlar.
			* ```RouteView``` ise ```AuthorizeRouteView``` olarak değişmiştir. Bu Authenticate olmamış kişilerin sayfayı görüntülememesini sağlar.
			* "DefaultLayout" kısmında ise site içerinde kullanacağımız layout bulunmaktadır. Bu dosya aynı proje içerisinde Shared klasöründe bulunmaktadır.
			* Auth taglerini kullanmak için ya yukardan "@using Microsoft.AspNetCore.Components.Authorization" diye eklemeliyiz, ya da "_Imports.razor" dosyasına aynı satırı eklemeliyiz. Aradaki fark _Imports dosyasına eklenen using statementlar tüm projede otomatik olarak import edilir. Sayfaya özgü değilse Imports'a ekleyelim.
		
		* İki taraf arasında kullanıcının kayıt ve giriş işlemleri yapabilmesi için 4 adet class'a ihtiyaç duyulmaktadır.
			* [LoginDto](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-52d719669ab2a06225d8c5fe4bd4d81105e6351490fe0430190e54f14490a40c). Login olurken kullanılacak.
			* [LoginResultDto](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-f72744424be119b42990e996e4d99f44a7d27683c7437008dda1c440a8c22f54). Loginimiz başarılı olunca bilgilerimizi tutacak.
			* [CurrentUser](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-2b8b3614080a9e519de8628a4a203649d17a1a35580960fc6a65097f87c150fc). Anlık olarak aktif kullanıcıyı ve claimlerini tutacak.
			* [RegisterDto](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-ceeb7f38e0bd2bfdbc4de293eb9f30b23c50c08e5a91e361fe048573a98f503b). Kayıt olurken kullanılacak
		
		* Authentication State'imi kontrol edecek bir [class](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-79244bd78ff589041ff9927c4baa37c06c82d1373c7c9709007a224c2312b635) gerekli. Bu class üzerinden başarılı giriş yapılıp yapılmadığı kontrol edilecek. Client'a özgü.
		
		* Client ile API arasında iletişim kuracak bir adet servise ihtiyacımız var.
			* Öncelikle tüm servislerde kullanacağımız bir [BaseClass](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-d738671844f2b7e34a2ba530db518a89bc34af4ce2fd5095e52d960fa0870c2e) gerekli.
			* Sonrasında ise önce [Interface](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-bd93aec4f9db665bed6af52ed8e6f18514d719397602c1f490892ff85d417342)'i ve sonrasında [implementasyonları içeren class](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-1fa6cf53584a04bc9d64fcefd650cbba8de7bc79bc9ff3eef460e7acbd7cb716) yazıldı. *[Düzeltme](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-1fa6cf53584a04bc9d64fcefd650cbba8de7bc79bc9ff3eef460e7acbd7cb716).
			* Oluşturulan bu servisi ve AuthStateProvider'ı Blazor'a Dependency Injection ile [ekliyoruz](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-b9a953e6792fac26b17c9d0ac814bfb5e097fccc65df8a6b31a936c480d88d0c).
			* ```wwwroot``` klasörü içine bir appsettings.json dosyası oluşturup AuthServer ve API projelerinin çalışacağı linkleri [ekleyelim](https://github.com/ktaze1/BlazorBoilerplate/commit/cee019781b110725c0362a1f2d4b805bd28e55a7#diff-3f1a32e11ed70dca5ebff373171d506b43fc7e4f58827135e06125623b996948).

		* AuthServer'a istek gelebilmesi için bir controller [oluşturulmuştur](https://github.com/ktaze1/BlazorBoilerplate/commit/6baf3d27dbef8fcd01a4dccdaa6b05bf0ef4cb57#diff-32b76d2f7db61b595fdfe7afcbec74fe6957a993450db36be4aaea0fba31fd2f)
			* Startup içerisinde controller'ları Map'lememiz gerektiğini özellikle [berlirtmemiz gerekir](https://github.com/ktaze1/BlazorBoilerplate/commit/6baf3d27dbef8fcd01a4dccdaa6b05bf0ef4cb57#diff-963b7276df42fb0d6f348b0a7553e3823485734a7652cf8221e35a54ba14de77R130) yoksa Route'larımız doğru olsa da Blazor Controller'ları görmeyecektir.
			* Controller'ı oluşturduktan sonra Kayıt ve Giriş işlemlerini yapacak şekilde [düzenlendi](https://github.com/ktaze1/BlazorBoilerplate/commit/4ca76c0e8b6efb57add5ce323a7017c8af94bf37#diff-32b76d2f7db61b595fdfe7afcbec74fe6957a993450db36be4aaea0fba31fd2f)
			* Identity Server'ı kullanbilmemiz için IdentityUser'ı inherit alan bir [class tanımlamalı](https://github.com/ktaze1/BlazorBoilerplate/commit/4ca76c0e8b6efb57add5ce323a7017c8af94bf37#diff-4509c921946a37fcb2e975becc251f7cd61344a201140e1b3b232e5d063fefd6) sonrasında ise bunu DBContext'te [göstermeliyiz](https://github.com/ktaze1/BlazorBoilerplate/commit/4ca76c0e8b6efb57add5ce323a7017c8af94bf37#diff-3926ffa2b9fa3427da2c49f2645622154e03126b565285426f6aed7d888f48cdR17)
			* Yeni bir migration ekleyip Identity Server Tablolarındaki değişiklikleri yaptıktan sonra kayıt olabilecek durumda olunacak.

		* Authenticate olarak giriş yapılabilmesi için öncelikle Authenticate olmadığımız durumlarda bizi login sayfasına yönelendirecek bir yapı kurulmalı.
			* Bunu için şöyle bir yol izlenebilir. Yukarda her sayfada belirlediğimiz layout'un ```MainLayout``` olduğunu öğrenmiştik. AuthenticationState bilgimizi bu Layout içine yazarak her sayfamızda bir state kontrolü [yapabiliriz](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-9bb397bb86471596311d66a04f1210b1327c23d283acd96a73f1545601f3744d).
			* Burada kullanıcı Authenticate olmadığı her durumda "login" endpoint'ine yönlendirilmektedir.
			* Bundan sonra şöyle bir problemimiz oluşacaktır: Eğer Authenticate State'i her sayfada kontrol ediyorsak, Login sayfasında da bunu kontrol eder ve giriş yapmadığımız için tekrardan login sayfasına yönlendiriliriz. Bunu önlemek için ise login ve register sayfalarına özgü, authentication state'i kontrol etmediğimiz yeni bir [Layout tanımlamalı](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-f307ba288bac93ae2d9b429a2ebd4fc46aa214fc73f46c887339f30f380d792a) ve bunu o sayfalarda [belirtmeliyiz](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-0fbfbbf73d94cab3610caba9b2ed5f582bb1c617b2751ad54d782205b3d2709dR2).

		* Birer [Login](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-0fbfbbf73d94cab3610caba9b2ed5f582bb1c617b2751ad54d782205b3d2709d) ve [Register](https://github.com/ktaze1/BlazorBoilerplate/commit/f3e4e6d16aae6845ef29b2aebf43b17de6365a8a#diff-a3eaa85b0a8a9b91bd698c4b49306f762148f4b49549a4af44df9cf13bf25dd7) sayfası oluşturduktan sonra proje ana hatlarıyla çalışır durumda.

		* AuthServer ve AdminUI projeleri beraber çalıştırılarak uygulama başlatılır.
