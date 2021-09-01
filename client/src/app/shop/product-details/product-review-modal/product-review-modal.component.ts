import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AuthDataService } from 'src/app/core';
import { IReview } from 'src/app/shared/models/product';

@Component({
  selector: 'app-product-review-modal',
  templateUrl: './product-review-modal.component.html',
  styleUrls: ['./product-review-modal.component.scss'],
})
export class ProductReviewModalComponent implements OnInit {
  @Input() addReviewToProduct = new EventEmitter();
  productName: string;
  reviewScore = 3;
  reviewerName: string;
  reviewDescription = '';
  review: IReview;

  constructor(
    public bsModalRef: BsModalRef,
    private authDataService: AuthDataService
  ) {}

  ngOnInit(): void {
    this.authDataService.user$.subscribe(
      (user) => (this.reviewerName = user?.displayName ?? 'Anonymous')
    );
  }

  addReviewAndCloseModal() {
    this.review = {
      productId: 0,
      reviewText: this.reviewDescription,
      reviewerName: this.reviewerName,
      created: new Date(),
      rating: this.reviewScore,
    };
    this.addReviewToProduct.emit(this.review);
    this.bsModalRef.hide();
  }
}
