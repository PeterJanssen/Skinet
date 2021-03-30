import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopComponent } from './shop.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { SharedModule } from '../shared/shared.module';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { ShopRoutingModule } from './shop-routing.module';
import { ProductReviewModalComponent } from './product-details/product-review-modal/product-review-modal.component';

@NgModule({
  declarations: [
    ShopComponent,
    ProductItemComponent,
    ProductDetailsComponent,
    ProductReviewModalComponent,
  ],
  imports: [CommonModule, SharedModule, ShopRoutingModule],
  exports: [ProductItemComponent],
})
export class ShopModule {}
