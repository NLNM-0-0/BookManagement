use master
drop database WPFBookManagement

create database WPFBookManagement
go

use WPFBookManagement
go

create table CHUCNANG
(
	MaChucNang varchar(4) primary key, 
	TenChucNang nvarchar(50),
)
create table NHOMNGUOIDUNG
(
	MaNhomNguoiDung varchar(8) primary key, 
	TenNhomNguoiDung nvarchar(50),
)
create table PHANQUYEN
(
	MaNhomNguoiDung varchar(8) foreign key references NHOMNGUOIDUNG(MaNhomNguoiDung),
	MaChucNang varchar(4) foreign key references CHUCNANG(MaChucNang),
	primary key(MaNhomNguoiDung, MaChucNang)
)
create table NHANVIEN
(
	MaNhanVien varchar(8) primary key,
	TenNhanVien nvarchar(100) not null,   
	GioiTinh nvarchar(4) check (GioiTinh = N'Nữ' or GioiTinh = N'Nam' or GioiTinh = N'Khác'),
	NgaySinh smalldatetime,
	DienThoai varchar(13),
	DiaChi nvarchar(200),
	NgayVaoLam smalldatetime not null,
	UserName varchar(50) not null unique,
	Password varchar(100),
	MaNhomNguoiDung varchar(8) foreign key references NHOMNGUOIDUNG(MaNhomNguoiDung),
	isActive bit not null,
)
create table KHACHHANG
(
	MaKhachHang varchar(8) primary key,
	TenKhachHang nvarchar(100) not null,   
	GioiTinh nvarchar(4) check (GioiTinh = N'Nữ' or GioiTinh = N'Nam' or GioiTinh = N'Khác'),
	NgaySinh smalldatetime,
	DienThoai varchar(13) not null unique,	
	DiaChi nvarchar(200),
	Email varchar(200),
	TienNo money default 0,
)
create table THELOAI
(
	MaTheLoai varchar(8) primary key,
	TenTheLoai nvarchar(100) unique,
)
go
create table TACGIA
(
	MaTacGia varchar(8) primary key,
	TenTacGia nvarchar(100) unique,
)
go
create table DAUSACH
(
	MaDauSach varchar(8) primary key,
	TenSach nvarchar(100),
	MaTheLoai varchar(8) foreign key references THELOAI(MaTheLoai)
)
go
create table CHITIETTACGIA
(
	MaTacGia varchar(8) foreign key references TACGIA(MaTacGia),
	MaDauSach varchar(8) foreign key references DAUSACH(MaDauSach),
	primary key(MaTacGia, MaDauSach)
)
go
create table SACH
(
	MaSach varchar(8) primary key,
	MaDauSach varchar(8) foreign key references DAUSACH(MaDauSach),
	NhaXuatBan nvarchar(100) not null,
	LanTaiBan int not null,
	SoLuong int default 0, 
	DonGiaNhapMoiNhat money not null,
	IsActive bit not null, 
)
go
create table PHIEUNHAPSACH
(
	MaPhieuNhap varchar(8) primary key,
	NgayNhap smalldatetime,
	MaNhanVien varchar(8) foreign key references NHANVIEN(MaNhanVien),
	TongTien money not null,
)
go
create table CHITIETPHIEUNHAP
(
	MaSach varchar(8) foreign key references SACH(MaSach),
	MaPhieuNhap varchar(8) foreign key references PHIEUNHAPSACH(MaPhieuNhap),	
	SoLuong int not null,
	DonGiaNhap money not null,
	primary key(MaSach, MaPhieuNhap)
)
go
create table HOADON
(
	MaHoaDon varchar(8) primary key,
	MaNhanVien varchar(8) foreign key references NHANVIEN(MaNhanVien),
	MaKhachHang varchar(8) foreign key references KHACHHANG(MaKhachHang),
	NgayLapHoaDon smalldatetime not null,
	TongTien money not null, 
	SoTienNo money not null,
)
go
create table CHITIETHOADON
(
	MaHoaDon varchar(8) foreign key references HOADON(MaHoaDon),	
	MaSach varchar(8) foreign key references SACH(MaSach),
	SoLuong int not null,
	DonGia money not null,
	primary key(MaHoaDon, MaSach)
)
go

create table BAOCAOTON
(
	MaBaoCaoTon varchar(8) primary key,
	Thang smalldatetime not null,
)
go
create table CHITIETBAOCAOTON
(
	MaBaoCaoTon varchar(8) foreign key references BAOCAOTON(MaBaoCaoTon),
	MaSach varchar(8) foreign key references SACH(MaSach),
	TonDau int not null,
	PhatSinh int not null,
	TonCuoi int not null,
	primary key(MaBaoCaoTon, MaSach)
)
go
create table BAOCAOCONGNO
(
	MaBaoCaoCongNo varchar(8) primary key,
	Thang smalldatetime not null,
)
go
create table CHITIETBAOCAOCONGNO
(
	MaBaoCaoCongNo varchar(8) foreign key references BAOCAOCONGNO(MaBaoCaoCongNo),
	MaKhachHang varchar(8) foreign key references KHACHHANG(MaKhachHang),
	NoDau money not null,
	PhatSinh money not null,
	NoCuoi money not null,
	primary key(MaBaoCaoCongNo, MaKhachHang)
)
go
create table PHIEUTHUNO
(
	MaPhieuThu varchar(8) primary key,
	NgayThu smalldatetime not null,
	SoTienThu money,
	MaNhanVien varchar(8) foreign key references NHANVIEN(MaNhanVien),
	MaKhachHang varchar(8) foreign key references KHACHHANG(MaKhachHang),
)
go

create table THAMSO
(
	TenThamSo nvarchar(100) primary key,
	GiaTri int not null
)
go

INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'0##kZd6a', N'Adharanad Finn')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'1##TG1u0', N'Arthur Conan Doyle')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'2##TG1d0', N'Ca Tây')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'2##TG123', N'Chan Ho Kei')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'2##TG134', N'Cung Kim Tuyến')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'1##xZd6e', N'Doãn Húc Thăng')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'6##nZd6q', N'Fujimaru')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'4##iZd6l', N'Guillem Balague')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'2##TGs22', N'Hà Mạt Bì')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'2##TG3d1', N'Happy Live Team')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'8##sZd6x', N'Higashino Keigo')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG3d1', N'Hữu Ngọc')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'7##hZd6t', N'Huỳnh Thái Ngọc')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'9##pZd6x', N'Miaki Sugaru')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG328', N'Morgan Housel')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'0##lZd6a', N'Natsuki Amasawa')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'7##fZd6s', N'Nguyễn Dương Tử')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG2u1', N'Nguyễn Nhật Ánh')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##aZd6i', N'Nguyễn Trà My')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##qZd6k', N'Nomura Mizuki')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'9##dZd6y', N'Ở Đây Zui Nè')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'1##yZd6d', N'Patrick King')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG3d2', N'Phạm Lê Liên')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG2u7', N'Robin Sharma')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'6##uZd6q', N'Stanton E Samenow')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG2u6', N'The Windy')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG2u9', N'Tống Khánh Thượng')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'3##TG2d5', N'Vincent Mallié')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'5##cZd6n', N'Vũ Ngọc Thuỷ')
INSERT [dbo].[TACGIA] ([MaTacGia], [TenTacGia]) VALUES (N'5##vZd6o', N'Wladislaw Jachtchenko')
GO
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'ApDungQDKiemTraTienThu', 1)
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'LuongSachNhapItNhat', 150)
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'LuongTonToiDaKhiBan', 20)
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'LuongTonToiDaKhiNhap', 300)
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'TienNoToiDaKhiBan', 1000000)
INSERT [dbo].[THAMSO] ([TenThamSo], [GiaTri]) VALUES (N'TiLeTinhDonGiaXuat', 105)
GO
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'3##QZd6i', N'Light novel')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'1##NZ9d8', N'Sách kỹ năng')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'1##NH9d6', N'Sách ngoại ngữ')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'4##SZd6l', N'Tâm lý học ')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'0##PZd6b', N'Thể thao')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'2##NH9d6', N'Tiểu thuyết')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'4##MZd6k', N'Truyện cổ tích')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'5##NZd6p', N'Truyện cười')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'3##NK9d7', N'Truyện trinh thám')
INSERT [dbo].[THELOAI] ([MaTheLoai], [TenTheLoai]) VALUES (N'3##NK910', N'Từ điển')
GO

INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'0##Bad6a', N'Tâm Lý Học Đàm Phán', N'4##SZd6l')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'0##lZd6a', N'Cổ Tích Muôn Màu - Sọ Dừa', N'4##MZd6k')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'1##Dad6d', N'Tâm Lý Học Ứng Dụng', N'4##SZd6l')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'1##uZd6d', N'Thỏ Bảy Màu Và Những Người Nghĩ Nó Là Bạn', N'5##NZd6p')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##5ad6h', N'Ba Ngày Hạnh Phúc', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##6ad6f', N'Cô gái văn chương và tên hề thích chết', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##8ad6g', N'Bạch Dạ Hành', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u1', N'Cho Tôi Xin Một Vé Đi Tuổi Thơ', N'2##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u2', N'Những Người Hàng Xóm', N'2##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u3', N'Làm Bạn Với Bầu Trời', N'2##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u4', N'Ra Bờ Suối Ngắm Hoa Kèn Hồng', N'2##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u5', N'Còn Chút Gì Để Nhớ', N'2##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u7', N'Vườn Hoa Mạt Dược Ký Sự - Những Kỳ Án Nổi Tiếng Chưa Có Lời Giải', N'3##NK9d7')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u8', N'Người Trong Lưới', N'3##NK9d7')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS2u9', N'Vụ Án Đầu Tiên Của Sherlock Holmes: Cuộc Điều Tra Màu Đỏ', N'3##NK9d7')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u0', N'Tự Học 2000 Từ Vựng Tiếng Anh Theo Chủ Đề', N'1##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u1', N'Your Very First English - Tự Học Nghe Nói Tiếng Anh Chuẩn Dễ Nhanh', N'1##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u2', N'Restart Your English - Daily Life - Yêu Lại Tiếng Anh Từ Đầu', N'1##NH9d6')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u3', N'Đời Ngắn Đừng Ngủ Dài', N'1##NZ9d8')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u4', N'Tâm Lý Học Về Tiền', N'1##NZ9d8')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u5', N'Thần Số Học: Thấu Hiểu Nhân Tâm', N'1##NZ9d8')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u6', N'Càng Kỷ Luật, Càng Tự Do', N'1##NZ9d8')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u7', N'Sổ Tay Thực Hành 66 Ngày Thử Thách', N'1##NZ9d8')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u8', N'Từ Điển Oxford Anh - Anh - Việt', N'3##NK910')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS3u9', N'Từ Điển Tiếng Việt Thông Dụng', N'3##NK910')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS4u0', N'Từ Điển Văn Hóa Cổ Truyền Việt Nam', N'3##NK910')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##DS4u1', N'Từ Điển Cơ Khí Và Máy Xây Dựng Anh - Việt', N'3##NK910')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##nZd6h', N'Cổ Tích Muôn Màu - Thạch Sanh', N'4##MZd6k')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##pZd6h', N'Vui Vẻ Không Quạu Nha', N'5##NZd6p')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##qZd6g', N'Một Cuốn Sách Buồn… Cười', N'5##NZd6p')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'2##sZd6g', N'Tí Đù - Dân Chơi Xóm', N'5##NZd6p')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'3##3ad6j', N'Tâm Lý Học Tội Phạm - Tập 2', N'4##SZd6l')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'3##kZd6j', N'Cổ Tích Muôn Màu - Tấm Cám', N'4##MZd6k')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'3##xZd6j', N'Chạy Bộ Cùng Người Kenya', N'0##PZd6b')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'5##0ad6o', N'Phía Sau Nghi Can X', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'6##1ad6q', N'Tâm Lý Học Tội Phạm - Tập 1', N'4##SZd6l')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'6##vZd6q', N'Lionel Messi - Hành Trình Của Một Thiên Tài', N'0##PZd6b')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'8##yZd6w', N'Và Rồi, Tháng 9 Không Có Cậu Đã Tới', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'9##=Zd6z', N'Thần Chết Làm Thêm 300 Yên/Giờ', N'3##QZd6i')
INSERT [dbo].[DAUSACH] ([MaDauSach], [TenSach], [MaTheLoai]) VALUES (N'9##4ad6x', N'Thuật Thao Túng - Góc Tối Ẩn Sau Mỗi Câu Nói', N'4##SZd6l')
GO

INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'0##kZd6a', N'3##xZd6j')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'0##lZd6a', N'8##yZd6w')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'1##TG1u0', N'2##DS2u9')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'1##xZd6e', N'0##Bad6a')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'1##yZd6d', N'1##Dad6d')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'2##TG123', N'2##DS2u8')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'2##TG134', N'2##DS4u1')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'2##TG1d0', N'2##DS3u6')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'2##TG3d1', N'2##DS3u7')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'2##TGs22', N'2##DS2u7')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##aZd6i', N'0##lZd6a')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##aZd6i', N'2##nZd6h')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##aZd6i', N'3##kZd6j')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##qZd6k', N'2##6ad6f')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2d5', N'2##DS2u9')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u1', N'2##DS2u1')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u1', N'2##DS2u2')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u1', N'2##DS2u3')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u1', N'2##DS2u4')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u1', N'2##DS2u5')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u6', N'2##DS3u0')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u6', N'2##DS3u1')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u6', N'2##DS3u2')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u6', N'2##DS3u8')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u7', N'2##DS3u3')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG2u9', N'2##DS3u5')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG328', N'2##DS3u4')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG3d1', N'2##DS4u0')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'3##TG3d2', N'2##DS3u9')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'4##iZd6l', N'6##vZd6q')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'5##cZd6n', N'0##lZd6a')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'5##cZd6n', N'2##nZd6h')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'5##cZd6n', N'3##kZd6j')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'5##vZd6o', N'9##4ad6x')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'6##nZd6q', N'9##=Zd6z')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'6##uZd6q', N'3##3ad6j')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'6##uZd6q', N'6##1ad6q')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'7##fZd6s', N'2##sZd6g')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'7##hZd6t', N'1##uZd6d')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'8##sZd6x', N'2##8ad6g')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'8##sZd6x', N'5##0ad6o')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'9##dZd6y', N'2##pZd6h')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'9##dZd6y', N'2##qZd6g')
INSERT [dbo].[CHITIETTACGIA] ([MaTacGia], [MaDauSach]) VALUES (N'9##pZd6x', N'2##5ad6h')
GO

INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'0##sZd6a', N'2##sZd6g', N'NXB Dân Trí', 199, 105000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'1##=Zd6e', N'9##=Zd6z', N'NXB Thế Giới', 150, 119000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'1##4ad6c', N'9##4ad6x', N'NXB Thế Giới', 299, 139000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'1##nZd6e', N'2##nZd6h', N'NXB Mỹ Thuật', 147, 15000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d1', N'2##DS2u1', N'NXB Trẻ', 148, 64000.0000, 1, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d2', N'2##DS2u2', N'NXB Trẻ', 149, 5600.0000, 2, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d3', N'2##DS2u3', N'NXB Trẻ', 196, 72000.0000, 1, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d4', N'2##DS2u4', N'NXB Trẻ', 149, 68000.0000, 1, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d5', N'2##DS2u5', N'NXB Trẻ', 158, 79000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d6', N'2##DS2u7', N'NXB Thế Giới', 148, 116000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d7', N'2##DS2u8', N'NXB Hồng Đức', 146, 123000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d8', N'2##DS2u9', N'NXB Hà Nội', 149, 141000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS2d9', N'2##DS3u0', N'NXB Đại Học Quốc Gia Hà Nội', 250, 39000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d0', N'2##DS3u1', N'NXB Hồng Đức', 250, 42000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d1', N'2##DS3u2', N'NXB Hồng Đức', 249, 118000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d2', N'2##DS3u3', N'NXB Trẻ', 245, 60000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d3', N'2##DS3u4', N'NXB Dân Trí', 250, 45000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d4', N'2##DS3u5', N'NXB Công Thương', 249, 39000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d5', N'2##DS3u6', N'NXB Thế Giới', 249, 39000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d6', N'2##DS3u7', N'NXB Văn hóa Văn nghệ', 98, 45000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d7', N'2##DS3u8', N'NXB Đại Học Quốc Gia Hà Nội', 100, 73000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d8', N'2##DS3u9', N'NXB Hồng Đức', 99, 68000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS3d9', N'2##DS4u0', N'NXB Kim Đồng', 99, 52000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'2##SS4d0', N'2##DS4u1', N'NXB Thanh Niên', 99, 60000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'3##uZd6i', N'1##uZd6d', N'NXB Dân Trí', 149, 99000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'4##5ad6m', N'2##5ad6h', N'NXB Hồng Đức', 198, 85000.0000, 1, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'4##8ad6l', N'2##8ad6g', N'NXB Hội Nhà Văn', 298, 209000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'4##Dad6m', N'1##Dad6d', N'NXB ĐH Kinh Tế Quốc Dân', 200, 129000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'4##pZd6m', N'2##pZd6h', N'NXB Phụ Nữ Việt Nam', 298, 69000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'4##qZd6l', N'2##qZd6g', N'NXB Phụ Nữ Việt Nam', 146, 70000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'5##xZd6o', N'3##xZd6j', N'NXB Thế Giới', 146, 150000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'5##yZd6p', N'8##yZd6w', N'NXB Thế Giới', 149, 106000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'7##0ad6t', N'5##0ad6o', N'NXB Hội Nhà Văn', 149, 109000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'7##3ad6t', N'3##3ad6j', N'NXB Đại học Kinh Tế Quốc Dân', 150, 139000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'7##6ad6u', N'2##6ad6f', N'NXB Văn Học', 139, 69000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'7##Bad6s', N'0##Bad6a', N'NXB Thanh Hóa', 150, 95000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'8##1ad6v', N'6##1ad6q', N'NXB Đại học Kinh Tế Quốc Dân', 200, 149000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'9##kZd6x', N'3##kZd6j', N'NXB Mỹ Thuật', 145, 15000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'9##lZd6y', N'0##lZd6a', N'NXB Mỹ Thuật', 198, 15000.0000, 0, 1)
INSERT [dbo].[SACH] ([MaSach], [MaDauSach], [NhaXuatBan], [SoLuong], [DonGiaNhapMoiNhat], [LanTaiBan], [IsActive]) VALUES (N'9##vZd6z', N'6##vZd6q', N'NXB Lao Động', 247, 339000.0000, 0, 1)
GO

INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn01', N'Phân quyền')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn02', N'Lập hóa đơn bán sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn03', N'Tra cứu sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn04', N'Đổi trạng thái bán của sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn05', N'Tra cứu phiếu nhập sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn06', N'Lập phiếu nhập sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn07', N'Tra cứu hóa đơn bán sách')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn08', N'Tra cứu thông tin khách hàng')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn09', N'Thay đổi thông tin khách hàng')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn10', N'Lập phiếu thu tiền')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn11', N'Tra cứu phiếu thu tiền')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn12', N'Lập báo cáo tháng')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn13', N'Thay đổi quy định')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn14', N'Thay đổi mật khẩu')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn15', N'Tra cứu nhân viên')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn16', N'Đổi thông tin nhân viên')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn17', N'Đổi trạng thái hoạt động nhân viên')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn18', N'Khôi phục mật khẩu nhân viên')
INSERT [dbo].[CHUCNANG] ([MaChucNang], [TenChucNang]) VALUES (N'cn19', N'Thêm người dùng')
GO

INSERT [dbo].[KHACHHANG] ([MaKhachHang], [TenKhachHang], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [Email], [TienNo]) VALUES (N'1##DZd6f', N'Nguyễn Thị Tuyết Nga', N'Nữ', CAST(N'2023-06-01T00:00:00' AS SmallDateTime), N'0585885222', N'TPHCM', N'nga@gmail.com', 490000.0000)
INSERT [dbo].[KHACHHANG] ([MaKhachHang], [TenKhachHang], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [Email], [TienNo]) VALUES (N'2##HZd6f', N'Trần Ngọc Nga', N'Nữ', CAST(N'2023-06-01T00:00:00' AS SmallDateTime), N'0989123456', N'TPHCM', N'trnga@gmail.com', 800000.0000)
INSERT [dbo].[KHACHHANG] ([MaKhachHang], [TenKhachHang], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [Email], [TienNo]) VALUES (N'8##IZd6w', N'Kim Thị Phú', N'Khác', CAST(N'2023-06-08T00:00:00' AS SmallDateTime), N'0787887123', N'TPHCM', N'phu@gmail.com', 1758000.0000)
INSERT [dbo].[KHACHHANG] ([MaKhachHang], [TenKhachHang], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [Email], [TienNo]) VALUES (N'9##FZd6x', N'Trần Hải Nam', N'Nam', CAST(N'2023-06-02T00:00:00' AS SmallDateTime), N'0902456723', N'Thủ Đức', N'nam@gmail.com', 500600.0000)
GO
INSERT [dbo].[NHOMNGUOIDUNG] ([MaNhomNguoiDung], [TenNhomNguoiDung]) VALUES (N'nnd00001', N'Quản trị viên')
INSERT [dbo].[NHOMNGUOIDUNG] ([MaNhomNguoiDung], [TenNhomNguoiDung]) VALUES (N'nnd00002', N'Nhân viên')
GO
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn01')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn02')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn03')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn04')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn05')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn06')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn07')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn08')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn09')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn10')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn11')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn12')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn13')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn14')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn15')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn16')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn17')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn18')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00001', N'cn19')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn02')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn03')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn04')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn05')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn06')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn07')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn08')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn09')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn10')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn11')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn12')
INSERT [dbo].[PHANQUYEN] ([MaNhomNguoiDung], [MaChucNang]) VALUES (N'nnd00002', N'cn14')
GO
INSERT [dbo].[NHANVIEN] ([MaNhanVien], [TenNhanVien], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [NgayVaoLam], [UserName], [Password], [MaNhomNguoiDung], [isActive]) VALUES (N'0##FZd6a', N'Nguyễn Văn Anh', N'Nữ', CAST(N'2023-06-02T00:00:00' AS SmallDateTime), N'0585885144', N'TPHCM', CAST(N'2022-12-10T22:04:00' AS SmallDateTime), N'user01', N'10511623978110401932407270171199124105146253', N'nnd00002', 1)
INSERT [dbo].[NHANVIEN] ([MaNhanVien], [TenNhanVien], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [NgayVaoLam], [UserName], [Password], [MaNhomNguoiDung], [isActive]) VALUES (N'1##HZd6d', N'Hạ Đăng Chí', N'Nam', CAST(N'2023-06-02T00:00:00' AS SmallDateTime), N'0919767632', N'Thủ Đức', CAST(N'2022-06-20T22:04:00' AS SmallDateTime), N'user02', N'10511623978110401932407270171199124105146253', N'nnd00002', 0)
INSERT [dbo].[NHANVIEN] ([MaNhanVien], [TenNhanVien], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [NgayVaoLam], [UserName], [Password], [MaNhomNguoiDung], [isActive]) VALUES (N'2##IZd6h', N'Đoàn Thị Hà', N'Nữ', CAST(N'2023-06-02T00:00:00' AS SmallDateTime), N'0909028451', N'Thủ Đức', CAST(N'2023-01-10T22:05:00' AS SmallDateTime), N'user03', N'10511623978110401932407270171199124105146253', N'nnd00002', 1)
INSERT [dbo].[NHANVIEN] ([MaNhanVien], [TenNhanVien], [GioiTinh], [NgaySinh], [DienThoai], [DiaChi], [NgayVaoLam], [UserName], [Password], [MaNhomNguoiDung], [isActive]) VALUES (N'user01', N'Nguyễn Văn A', N'Nữ', CAST(N'2000-01-01T00:00:00' AS SmallDateTime), N'01234567890', N'Dia chi t ne', CAST(N'2023-05-17T00:00:00' AS SmallDateTime), N'admin', N'2251022057731868917119086224872421513662', N'nnd00001', 1)
GO

INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'1##QZd6e', CAST(N'2022-05-09T22:10:00' AS SmallDateTime), N'0##FZd6a', 41700000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##MZd6i', CAST(N'2022-05-08T22:10:00' AS SmallDateTime), N'user01', 38700000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##PN0u6', CAST(N'2022-02-08T00:00:00' AS SmallDateTime), N'2##IZd6h', 55840000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##PN0u7', CAST(N'2022-03-15T00:00:00' AS SmallDateTime), N'2##IZd6h', 57000000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##PN0u8', CAST(N'2022-04-11T00:00:00' AS SmallDateTime), N'2##IZd6h', 39000000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##PN0u9', CAST(N'2022-05-17T00:00:00' AS SmallDateTime), N'2##IZd6h', 27000000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'3##PN1u0', CAST(N'2022-05-16T00:00:00' AS SmallDateTime), N'1##HZd6d', 54600000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'6##NZd6p', CAST(N'2022-05-10T22:10:00' AS SmallDateTime), N'user01', 283250000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'6##SZd6q', CAST(N'2022-05-10T22:10:00' AS SmallDateTime), N'0##FZd6a', 40050000.0000)
INSERT [dbo].[PHIEUNHAPSACH] ([MaPhieuNhap], [NgayNhap], [MaNhanVien], [TongTien]) VALUES (N'8##PZd6w', CAST(N'2022-05-10T22:10:00' AS SmallDateTime), N'user01', 50650000.0000)
GO
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'0##sZd6a', N'6##NZd6p', 200, 105000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'1##=Zd6e', N'6##NZd6p', 150, 119000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'1##4ad6c', N'1##QZd6e', 300, 139000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'1##nZd6e', N'3##MZd6i', 150, 15000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d1', N'3##PN0u6', 150, 64000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d2', N'3##PN0u6', 150, 56000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d3', N'3##PN0u6', 200, 72000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d4', N'3##PN0u6', 150, 68000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d5', N'3##PN0u6', 160, 79000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d6', N'3##PN0u7', 150, 116000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d7', N'3##PN0u7', 150, 123000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d8', N'3##PN0u7', 150, 141000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d9', N'3##PN0u8', 100, 38000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS2d9', N'3##PN1u0', 150, 39000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d0', N'3##PN0u8', 100, 42000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d0', N'3##PN1u0', 150, 42000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d1', N'3##PN0u8', 100, 57000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d1', N'3##PN1u0', 150, 60000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d2', N'3##PN0u9', 100, 43000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d2', N'3##PN1u0', 150, 42000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d3', N'3##PN0u9', 100, 96000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d3', N'3##PN1u0', 150, 45000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d4', N'3##PN0u9', 100, 51000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d4', N'3##PN1u0', 150, 52000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d5', N'3##PN0u9', 100, 37000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d5', N'3##PN1u0', 150, 39000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d6', N'3##PN0u9', 100, 43000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d6', N'3##PN1u0', 150, 45000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d7', N'3##PN0u8', 100, 73000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d8', N'3##PN0u8', 100, 68000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS3d9', N'3##PN0u8', 100, 52000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'2##SS4d0', N'3##PN0u8', 100, 60000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'3##uZd6i', N'6##NZd6p', 150, 99000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'4##5ad6m', N'6##NZd6p', 200, 85000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'4##8ad6l', N'6##NZd6p', 300, 209000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'4##Dad6m', N'6##SZd6q', 200, 129000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'4##pZd6m', N'3##MZd6i', 300, 69000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'4##qZd6l', N'3##MZd6i', 150, 70000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'5##xZd6o', N'6##NZd6p', 150, 150000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'5##yZd6p', N'6##NZd6p', 150, 106000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'7##0ad6t', N'6##NZd6p', 150, 109000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'7##3ad6t', N'8##PZd6w', 150, 139000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'7##6ad6u', N'6##NZd6p', 150, 69000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'7##Bad6s', N'6##SZd6q', 150, 95000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'8##1ad6v', N'8##PZd6w', 200, 149000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'9##kZd6x', N'3##MZd6i', 150, 15000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'9##lZd6y', N'3##MZd6i', 200, 15000.0000)
INSERT [dbo].[CHITIETPHIEUNHAP] ([MaSach], [MaPhieuNhap], [SoLuong], [DonGiaNhap]) VALUES (N'9##vZd6z', N'6##NZd6p', 250, 339000.0000)
GO

INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'1##DZd6f', N'0##FZd6a', N'1##DZd6f', CAST(N'2023-05-20T22:12:00' AS SmallDateTime), 1134000.0000, 1000000.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'1##NZd6d', N'user01', NULL, CAST(N'2023-06-10T22:21:00' AS SmallDateTime), 325500.0000, 0.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'2##KZd6f', N'user01', N'2##HZd6f', CAST(N'2023-04-10T22:21:00' AS SmallDateTime), 1040550.0000, 1000000.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'4##IZd6k', N'0##FZd6a', N'9##FZd6x', CAST(N'2023-05-09T22:15:00' AS SmallDateTime), 797580.0000, 700000.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'4##MZd6m', N'user01', NULL, CAST(N'2023-06-01T22:21:00' AS SmallDateTime), 511350.0000, 0.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'5##FZd6o', N'0##FZd6a', N'9##FZd6x', CAST(N'2023-06-05T22:13:00' AS SmallDateTime), 327600.0000, 300600.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'8##HZd6w', N'0##FZd6a', N'9##FZd6x', CAST(N'2023-04-12T22:14:00' AS SmallDateTime), 791700.0000, 700000.0000)
INSERT [dbo].[HOADON] ([MaHoaDon], [MaNhanVien], [MaKhachHang], [NgayLapHoaDon], [TongTien], [SoTienNo]) VALUES (N'8##PZd6w', N'user01', N'8##IZd6w', CAST(N'2023-05-17T22:22:00' AS SmallDateTime), 1758750.0000, 1758000.0000)
GO
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##DZd6f', N'4##5ad6m', 1, 89250.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##DZd6f', N'4##8ad6l', 1, 219450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##DZd6f', N'5##xZd6o', 1, 157500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##DZd6f', N'7##6ad6u', 9, 72450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##DZd6f', N'9##lZd6y', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'1##nZd6e', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'2##SS2d3', 1, 75600.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'2##SS2d5', 1, 82950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'2##SS3d2', 1, 63000.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'7##6ad6u', 1, 72450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'1##NZd6d', N'9##kZd6x', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'2##SS2d1', 1, 67200.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'2##SS2d5', 1, 82950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'2##SS3d2', 2, 63000.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'4##qZd6l', 2, 73500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'5##xZd6o', 1, 157500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'7##6ad6u', 1, 72450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'9##kZd6x', 2, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'2##KZd6f', N'9##vZd6z', 1, 355950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'2##SS2d2', 1, 5880.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'2##SS2d4', 1, 71400.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'2##SS2d7', 1, 129150.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'2##SS3d6', 1, 47250.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'4##qZd6l', 1, 73500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'7##0ad6t', 1, 114450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##IZd6k', N'9##vZd6z', 1, 355950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'1##nZd6e', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'2##SS2d1', 1, 67200.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'2##SS2d3', 1, 75600.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'2##SS2d7', 2, 129150.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'2##SS3d2', 1, 63000.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'4##MZd6m', N'9##kZd6x', 2, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'5##FZd6o', N'1##nZd6e', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'5##FZd6o', N'2##SS2d3', 1, 75600.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'5##FZd6o', N'2##SS3d2', 1, 63000.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'5##FZd6o', N'5##xZd6o', 1, 157500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'5##FZd6o', N'9##lZd6y', 1, 15750.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##HZd6w', N'2##SS2d3', 1, 75600.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##HZd6w', N'2##SS2d7', 1, 129150.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##HZd6w', N'4##qZd6l', 1, 73500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##HZd6w', N'5##xZd6o', 1, 157500.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##HZd6w', N'9##vZd6z', 1, 355950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'0##sZd6a', 1, 110250.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'1##4ad6c', 1, 145950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS2d6', 2, 121800.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS2d8', 1, 148050.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d1', 1, 123900.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d4', 1, 40950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d5', 1, 40950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d6', 1, 47250.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d8', 1, 71400.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS3d9', 1, 54600.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'2##SS4d0', 1, 63000.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'3##uZd6i', 1, 103950.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'4##5ad6m', 1, 89250.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'4##8ad6l', 1, 219450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'4##pZd6m', 2, 72450.0000)
INSERT [dbo].[CHITIETHOADON] ([MaHoaDon], [MaSach], [SoLuong], [DonGia]) VALUES (N'8##PZd6w', N'5##yZd6p', 1, 111300.0000)
GO
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'0##KZd6b', CAST(N'2023-04-15T22:23:00' AS SmallDateTime), 700000.0000, N'user01', N'9##FZd6x')
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'2##DZd6g', CAST(N'2023-05-23T22:14:00' AS SmallDateTime), 400000.0000, N'0##FZd6a', N'1##DZd6f')
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'3##FZd6k', CAST(N'2023-06-01T22:14:00' AS SmallDateTime), 20000.0000, N'0##FZd6a', N'1##DZd6f')
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'3##IZd6j', CAST(N'2023-05-15T22:15:00' AS SmallDateTime), 500000.0000, N'0##FZd6a', N'9##FZd6x')
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'4##HZd6k', CAST(N'2023-06-10T22:14:00' AS SmallDateTime), 90000.0000, N'0##FZd6a', N'1##DZd6f')
INSERT [dbo].[PHIEUTHUNO] ([MaPhieuThu], [NgayThu], [SoTienThu], [MaNhanVien], [MaKhachHang]) VALUES (N'7##MZd6t', CAST(N'2023-06-20T22:24:00' AS SmallDateTime), 200000.0000, N'user01', N'2##HZd6f')
GO
