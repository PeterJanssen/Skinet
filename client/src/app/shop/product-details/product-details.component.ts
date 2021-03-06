import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryImageSize,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ProductReviewModalComponent } from './product-review-modal/product-review-modal.component';
import { IProduct, IReview } from 'src/app/shared';
import { BasketDataService, ProductDataService } from 'src/app/core';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit {
  product: IProduct;
  quantity = 1;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  bsModalRef: BsModalRef;
  isReadonly = true;

  constructor(
    private productDataService: ProductDataService,
    private basketDataService: BasketDataService,
    private activatedRoute: ActivatedRoute,
    private bcService: BreadcrumbService,
    private modalService: BsModalService
  ) {
    this.bcService.set('@productDetails', ' ');
  }

  ngOnInit(): void {
    this.loadProduct();
  }

  addItemToBasket(): void {
    this.basketDataService.addItemToBasket(this.product, this.quantity);
  }

  incrementQuantity(): void {
    this.quantity++;
  }

  decrementQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  loadProduct(): void {
    this.productDataService
      .getProduct(+this.activatedRoute.snapshot.paramMap.get('id'))
      .subscribe(
        (product) => {
          this.product = product;
          this.bcService.set('@productDetails', this.product.name);
          this.initializeGallery();
        },
        (error) => console.log(error)
      );
  }

  initializeGallery() {
    this.galleryOptions = [
      {
        width: '500px',
        height: '600px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Fade,
        imageSize: NgxGalleryImageSize.Contain,
        thumbnailSize: NgxGalleryImageSize.Contain,
        preview: false,
      },
    ];
    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.product.photos) {
      imageUrls.push({
        small: photo.pictureUrl,
        medium: photo.pictureUrl,
        big: photo.pictureUrl,
      });
    }
    return imageUrls;
  }

  addReviewToProduct(productName: string) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        productName,
      },
    };
    this.bsModalRef = this.modalService.show(
      ProductReviewModalComponent,
      config
    );
    this.bsModalRef.content.addReviewToProduct.subscribe((values: IReview) => {
      const review: IReview = {
        ...values,
      };
      review.productId = this.product.id;
      this.productDataService
        .addReviewToProduct(review)
        .subscribe((product: IProduct) => {
          this.product = product;
        });
    });
  }
}
