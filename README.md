## Hướng dẫn cài đặt
 **1. Tải Project về máy**
 
 **2. Thêm dữ liệu vào database**
 - Tải [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) và [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
 - Tạo SQL Server cho máy
*(có thể bỏ qua 2 bước trên nếu đã có SQL Server và SSMS trên máy)*
 - Khởi động phần mềm SQL Server Management Studio (SSMS) và chọn **Server name** muốn lưu dữ liệu
 - Mở file **database.sql** trong thư mục **BookManagement** vừa tải
 - Sau khi nhấn nút **Excute** hoặc **F5** để chạy đoạn code, dữ liệu đã được thêm thành công

**3. Thay đổi ServerName trong SourceCode**
- Mở project vừa tải trong Visual Studio 
- Vào file **App.config** sửa trường **data source** trong **connectionString** thành **Server name** vừa thêm dữ liệu

*Ví dụ:* nếu **Server name** thêm dữ liệu là *ASUS*

`<add name="WPFBookManagementEntities" connectionString="metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=DESKTOP;initial catalog=WPFBookManagement;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />`

được sửa lại thành 

`<add name="WPFBookManagementEntities" connectionString="metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=ASUS;initial catalog=WPFBookManagement;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />`
- **Build** project để hoàn tất

## Người đóng góp

Nhóm 3 lớp SE104.N21 trường Đại học Công nghệ Thông tin

|MSSV            |Họ và tên                      |Tên Github                         |
|----------------|-------------------------------|-----------------------------|
|&nbsp;&nbsp;&nbsp;21520095&nbsp;&nbsp;&nbsp;        |&nbsp;&nbsp;&nbsp;Bùi Vĩ Quốc&nbsp;&nbsp;&nbsp;                       |&nbsp;&nbsp;&nbsp;[bvquoc](https://github.com/bvquoc)          |
|&nbsp;&nbsp;&nbsp;21520243&nbsp;&nbsp;&nbsp;        |&nbsp;&nbsp;&nbsp;Vũ Hoàng&nbsp;&nbsp;&nbsp;                       |&nbsp;&nbsp;&nbsp;[vuhoang-gr](https://github.com/vuhoang-gr)          |
|&nbsp;&nbsp;&nbsp;21520339&nbsp;&nbsp;&nbsp;        |&nbsp;&nbsp;&nbsp;Nguyễn Lê Ngọc Mai&nbsp;&nbsp;&nbsp;             |&nbsp;&nbsp;&nbsp;[NLNM-0-0](https://github.com/NLNM-0-0)           |
|&nbsp;&nbsp;&nbsp;21520495&nbsp;&nbsp;&nbsp;        |&nbsp;&nbsp;&nbsp;Nguyễn Minh Trí&nbsp;&nbsp;&nbsp;                |&nbsp;&nbsp;&nbsp;[Karin1412](https://github.com/Karin1412)
|&nbsp;&nbsp;&nbsp;21521495&nbsp;&nbsp;&nbsp;        |&nbsp;&nbsp;&nbsp;Nguyễn Kim Anh Thư&nbsp;&nbsp;&nbsp;             |&nbsp;&nbsp;&nbsp;[kimthu09](https://github.com/kimthu09)
