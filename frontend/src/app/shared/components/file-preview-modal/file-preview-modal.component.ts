import { Component, Output, EventEmitter, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
// import { CommonConstant, CommonUtility } from "@app-core";
import { CommonConstant,CommonUtility } from "app/core";
import { Subject } from "rxjs";

@Component({
	selector: "file-preview-modal",
	templateUrl: "./file-preview-modal.component.html",
	styles: [`
		.iframe{
			width: 100%;
			min-height: 500px;			
			-moz-box-sizing: border-box;
			box-sizing: border-box;
		}
		h4{
			color:white;
		}
		img{
			max-width: 100%;
		}
		.spinEffect90{
			transform: rotate(90deg);
			-webkit-transform: rotate(90deg);
			-ms-transform: rotate(90deg);
		 }
		 .spinEffect180{
			transform: rotate(180deg);
			-webkit-transform: rotate(180deg);
			-ms-transform: rotate(180deg);
		 }
		 .spinEffect270{
			transform: rotate(270deg);
			-webkit-transform: rotate(270deg);
			-ms-transform: rotate(270deg);
		 }
	`]
})
export class FilePreviewModalComponent implements OnInit {

	file: any;
	type: "file" | "path";
	title: string;
	newfile: any;
	currentRotate: number = 1;
	isRotate: boolean = false;
	angle: number = 0;
	angleFile: number = 0;

	@Output()
	rotateDocument = new EventEmitter();

	public onClose: Subject<{ isImageRotate: boolean, newFile: any }>;

	constructor(public bsModalRef: BsModalRef) {
	}

	ngOnInit(): void {
		this.onClose = new Subject();
	}

	closePopup() {
		if (this.isRotate) {
			this.rotateDocument = this.newfile;
		}
		this.onClose.next({ isImageRotate: this.isRotate, newFile: this.rotateDocument });
		this.bsModalRef.hide();
	}

	setData(data: { file: any, title: string, type: "file" | "path" }) {
		this.file = data.file;
		this.type = data.type;
		this.title = data.title;

		if (CommonUtility.isNotEmpty(this.file) && CommonUtility.isString(this.file) &&
			(data.file.endsWith('.tif') || data.file.endsWith('.txt') || data.file.endsWith('.doc') || data.file.endsWith('.docx'))
		) {
			setTimeout(() => {
				if (data.file.endsWith('.txt')) {
					window.open(this.file, '_blank');
				}

				this.closePopup();
				return;
			}, 200);
		}
	}

	isImage(path: string): boolean {
		return CommonConstant.imageTypes.some(x => path.endsWith(x)) ||
			path.includes("data:image/jpg;base64") || path.includes("data:image/png;base64");
	}

	rotateImage() {
		var img = document.getElementById("image");

		this.angle = (this.angle + 90) % 360;
		// img.className = "spinEffect" + this.angle;
		// var canvas = document.getElementById('canvas');
		// this.rotateImageCanvas(this.angle, img, canvas);

		//this.currentRotate = this.angle == 0 ? 1 : this.angle == 90 ? 3 : this.angle == 180 ? 6 : 8;
		this.currentRotate = (this.currentRotate == 0 || this.currentRotate == 1) ? 8 : this.currentRotate == 8 ? 3 : this.currentRotate == 3 ? 6 : 1;

		// console.log(this.angle);
		// console.log(this.currentRotate);
		this.isRotate = true;
		this.rotateBase64Image(img, this.currentRotate);
		if (!this.isRotate) {
			img.className = "spinEffect" + this.angle;
		}
	}

	rotateImageFile() {
		var img = document.getElementById("imageFile");

		this.angleFile = (this.angleFile + 90) % 360;
		//img.className = "spinEffect" + this.angleFile;
		// if (this.currentRotate === 4) {
		// 	this.currentRotate = 1;
		// } else {
		// 	this.currentRotate += 1;
		// }

		//this.currentRotate = this.angleFile == 0 ? 1 : this.angleFile == 90 ? 8 : this.angleFile == 180 ? 6 : 8;
		this.currentRotate = (this.currentRotate == 0 || this.currentRotate == 1) ? 8 : this.currentRotate == 8 ? 3 : this.currentRotate == 3 ? 6 : 1;
		// console.log(this.angleFile);
		// console.log(this.currentRotate);
		this.isRotate = true;
		this.rotateBase64Image(img, this.currentRotate);
		if (!this.isRotate) {
			img.className = "spinEffect" + this.angleFile;
		}
	}

	rotateBase64Image(base64data, angle) {
		var canvas: any = document.createElement('canvas');
		var ctx = canvas.getContext('2d');
		var img = new Image();

		if (base64data.src.includes(";base64,")) {
			img.src = base64data.src;
			var width = img.width,
				height = img.height;

			if (4 < angle && angle < 9) {
				canvas.width = height;
				canvas.height = width;
			} else {
				canvas.width = width;
				canvas.height = height;
			}

			switch (angle) {
				case 2: ctx.transform(-1, 0, 0, 1, width, 0); break;
				case 3: ctx.transform(-1, 0, 0, -1, width, height); break;
				case 4: ctx.transform(1, 0, 0, -1, 0, height); break;
				case 5: ctx.transform(0, 1, 1, 0, 0, 0); break;
				case 6: ctx.transform(0, 1, -1, 0, height, 0); break;
				case 7: ctx.transform(0, -1, -1, 0, height, width); break;
				case 8: ctx.transform(0, -1, 1, 0, 0, width); break;
				default: break;
			}

			// draw image
			ctx.drawImage(img, 0, 0);

			this.file = canvas.toDataURL();
			this.newfile = this.file;
		}
		else {
			this.isRotate = false;
		}
	}

	// rotateImageCanvas(degree, img, canvas) {
	// 	if (document.getElementById('canvas')) {
	// 		var cContext = canvas.getContext('2d');
	// 		var cw = img.width, ch = img.height, cx = 0, cy = 0;

	// 		//   Calculate new canvas size and x/y coorditates for image
	// 		switch (degree) {
	// 			case 90:
	// 				cw = img.height;
	// 				ch = img.width;
	// 				cy = img.height * (-1);
	// 				break;
	// 			case 180:
	// 				cx = img.width * (-1);
	// 				cy = img.height * (-1);
	// 				break;
	// 			case 270:
	// 				cw = img.height;
	// 				ch = img.width;
	// 				cx = img.width * (-1);
	// 				break;
	// 		}

	// 		//  Rotate image            
	// 		canvas.setAttribute('width', cw);
	// 		canvas.setAttribute('height', ch);
	// 		cContext.rotate(degree * Math.PI / 180);
	// 		cContext.drawImage(img, cx, cy);
	// 		console.log(canvas.toDataURL());
	// 		// this is base64 of your image
	// 		// to get image run it with server like https://www.npmjs.com/package/serve
	// 		// $('#download').attr('href', canvas.toDataURL())
	// 	} else {
	// 		switch (degree) {
	// 			case 0:
	// 				img.style.filter = 'progid:DXImageTransform.Microsoft.BasicImage(rotation=0)';
	// 				break;
	// 			case 90:
	// 				img.style.filter = 'progid:DXImageTransform.Microsoft.BasicImage(rotation=1)';
	// 				break;
	// 			case 180:
	// 				img.style.filter = 'progid:DXImageTransform.Microsoft.BasicImage(rotation=2)';
	// 				break;
	// 			case 270:
	// 				img.style.filter = 'progid:DXImageTransform.Microsoft.BasicImage(rotation=3)';
	// 				break;
	// 		}
	// 	}
	// }
}