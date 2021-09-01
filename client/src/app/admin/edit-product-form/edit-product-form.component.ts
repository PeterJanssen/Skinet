import { Component, Input, OnInit } from '@angular/core';
import { ProductFormValues } from '../../shared/models/product';
import { ActivatedRoute, Router } from '@angular/router';
import { IBrand, IType } from 'src/app/shared';
import { ProductDataService } from 'src/app/core';

@Component({
  selector: 'app-edit-product-form',
  templateUrl: './edit-product-form.component.html',
  styleUrls: ['./edit-product-form.component.scss'],
})
export class EditProductFormComponent implements OnInit {
  @Input() product: ProductFormValues;
  @Input() brands: IBrand[];
  @Input() types: IType[];

  constructor(
    private route: ActivatedRoute,
    private productDataService: ProductDataService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  updatePrice(event: any) {
    this.product.price = event;
  }

  onSubmit(product: ProductFormValues) {
    if (this.route.snapshot.url[0].path === 'edit') {
      const updatedProduct = {
        ...this.product,
        ...product,
        price: +product.price,
      };
      this.productDataService
        .updateProduct(updatedProduct, +this.route.snapshot.paramMap.get('id'))
        .subscribe(() => {
          this.router.navigate(['/admin']);
        });
    } else {
      const newProduct = { ...product, price: +product.price };
      this.productDataService.createProduct(newProduct).subscribe(() => {
        this.router.navigate(['/admin']);
      });
    }
  }

  cancel() {
    this.router.navigate(['/admin']);
  }
}
