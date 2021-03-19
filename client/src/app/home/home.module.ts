import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { SharedModule } from '../shared/shared.module';
import { ShopModule } from '../shop/shop.module';

@NgModule({
  declarations: [HomeComponent],
  imports: [CommonModule, SharedModule, ShopModule],
  exports: [HomeComponent],
})
export class HomeModule {}
