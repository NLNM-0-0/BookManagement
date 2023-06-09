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
	MaNhomNguoiDung varchar(4) primary key, 
	TenNhomNguoiDung nvarchar(50),
)
create table PHANQUYEN
(
	MaNhomNguoiDung varchar(4) foreign key references NHOMNGUOIDUNG(MaNhomNguoiDung),
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
	MaNhomNguoiDung varchar(4) foreign key references NHOMNGUOIDUNG(MaNhomNguoiDung),
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
	TenSach nvarchar(100) unique,
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
	SoLuong int default 0, 
	DonGiaNhapMoiNhat money not null,
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

set dateformat dmy

insert into CHUCNANG
values('cn01', N'Phân quyền'),
('cn02', N'Lập phiếu nhập sách'),
('cn03', N'Lập hóa đơn bán sách'),
('cn04', N'Tra cứu sách'),
('cn05', N'Lập phiếu thu tiền'),
('cn06', N'Lập báo cáo tháng'),
('cn07', N'Tra cứu hóa đơn bán sách'),
('cn08', N'Tra cứu phiếu thu tiền'),
('cn09', N'Tra cứu phiếu nhập sách'),
('cn10', N'Lưu thông tin khách hàng'),
('cn11', N'Tra cứu thông tin khách hàng'),
('cn12', N'Thay đổi mật khẩu'),
('cn13', N'Tra cứu nhân viên'),
('cn14', N'Thêm người dùng'),
('cn15', N'Thay đổi quy định')	

insert into NHOMNGUOIDUNG
values('nnd1', N'Quản trị viên'),
('nnd2', N'Nhân viên')

insert into PHANQUYEN
values('nnd1', 'cn01'),
('nnd1', 'cn02'),
('nnd1', 'cn03'),
('nnd1', 'cn04'),
('nnd1', 'cn05'),
('nnd1', 'cn06'),
('nnd1', 'cn07'),
('nnd1', 'cn08'),
('nnd1', 'cn09'),
('nnd1', 'cn10'),
('nnd1', 'cn11'),
('nnd1', 'cn12'),
('nnd1', 'cn13'),
('nnd1', 'cn14'),
('nnd1', 'cn15'),
('nnd2', 'cn02'),
('nnd2', 'cn03'),
('nnd2', 'cn04'),
('nnd2', 'cn05'),
('nnd2', 'cn06'),
('nnd2', 'cn07'),
('nnd2', 'cn08'),
('nnd2', 'cn09'),
('nnd2', 'cn10'),
('nnd2', 'cn11'),
('nnd2', 'cn12'),
('nnd2', 'cn13')

insert into THAMSO
values('LuongSachNhapItNhat', 150),
('LuongTonToiDaKhiNhap', 30),
('TienNoToiDaKhiBan', 1000000),
('LuongTonToiDaKhiBan', 20),
('TiLeTinhDonGiaXuat', 105),
('ApDungQDKiemTraTienThu', 1)

insert into NHANVIEN
values('user01','Nguyen Van A', N'Nữ', '1/1/2000', '01234567890', 'Dia chi t ne', '17/5/2023', 'admin', '123456', 'nnd1', 1)
