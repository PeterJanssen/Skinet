import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IProduct, IReview } from 'src/app/shared/models/product';

@Component({
  selector: 'app-product-review-modal',
  templateUrl: './product-review-modal.component.html',
  styleUrls: ['./product-review-modal.component.scss'],
})
export class ProductReviewModalComponent implements OnInit {
  @Input() addReviewToProduct = new EventEmitter();
  productName: string;
  reviewScore = 3;
  reviewDescription = '';
  review: IReview;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  addReviewAndCloseModal() {
    this.review = {
      reviewText: this.reviewDescription,
      rating: this.reviewScore,
    };
    this.addReviewToProduct.emit(this.review);
    this.bsModalRef.hide();
  }
}
