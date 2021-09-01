import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { ProductDataService } from './product-data.service';
import {
  IProduct,
  IReview,
  ProductFormValues,
  ProductPagination,
  ProductParams,
} from 'src/app/shared';

describe('ProductDataService', () => {
  let productDataService: ProductDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.product;

  const testPaginatedProducts: ProductPagination = {
    count: 2,
    pageIndex: 1,
    pageSize: 2,
    data: [
      {
        id: 1,
        name: 'Angular Speedster Board 2000',
        description: 'Lorem ipsum dolor sit amet.',
        price: 200,
        quantity: 1,
        pictureUrl: 'images/products/sb-ang1.png',
        productType: 'Boards',
        productBrand: 'Angular',
        photos: [
          {
            id: 1,
            isMain: true,
            fileName: 'testOne',
            pictureUrl: 'images/products/sb-ang1.png',
          },
          {
            id: 2,
            isMain: false,
            fileName: 'testTwo',
            pictureUrl: 'images/products/sb-ang2.png',
          },
        ],
        reviews: [],
      },
      {
        id: 2,
        name: 'Green Angular Board 3000',
        description:
          'Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.',
        price: 150,
        quantity: 1,
        pictureUrl: 'images/products/sb-ang2.png',
        productType: 'Boards',
        productBrand: 'Angular',
        photos: [],
        reviews: [],
      },
    ],
  };

  const testProductFormValues: ProductFormValues = {
    name: 'test',
    description: '',
    price: 0,
    pictureUrl: '',
    productBrandId: 1,
    productTypeId: 1,
  };

  const testReview: IReview = {
    productId: 1,
    rating: 5,
    reviewText: 'Test',
    reviewerName: 'Admin',
    created: undefined,
  };

  const testProductWithTestReview: IProduct = {
    id: 1,
    name: 'Green Angular Board 3000',
    description: 'Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.',
    price: 150,
    quantity: 1,
    pictureUrl: 'images/products/sb-ang2.png',
    productType: 'Boards',
    productBrand: 'Angular',
    photos: [],
    reviews: [testReview],
  };

  const testProductParams: ProductParams = {
    brandId: 1,
    typeId: 1,
    pageNumber: 1,
    pageSize: 6,
    search: '',
    sort: 'name_asc',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductDataService],
    });

    productDataService = TestBed.inject(ProductDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(productDataService).toBeTruthy();
  });

  it('should set product params', () => {
    productDataService.setProductParams(testProductParams);
    const productParams: ProductParams = productDataService.getProductParams();

    expect(productParams).toEqual(testProductParams);
  });

  it('should PUT a new product image', () => {
    const file: File = new File([], 'test');
    productDataService.uploadImage(file, 1).subscribe();

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1/photo'
    );
    expect(productRequest.request.method).toEqual('PUT');

    productRequest.flush(testPaginatedProducts);

    httpTestingController.verify();
  });

  it('should GET all paginated products', () => {
    productDataService
      .getProducts()
      .subscribe((response: ProductPagination) => {
        expect(response.count).toBe(2);
        expect(response.pageSize).toBe(2);
        expect(response.pageIndex).toBe(1);
        expect(response.data).toBeTruthy();
        expect(response.data.length).toBe(2);
      });

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '?sort=name&pageIndex=1&pageSize=6'
    );
    expect(productRequest.request.method).toEqual('GET');

    productRequest.flush(testPaginatedProducts);

    httpTestingController.verify();
  });

  it('should GET a product', () => {
    productDataService.getProduct(1).subscribe((product: IProduct) => {
      expect(product).toBeTruthy();
      expect(product).toEqual(testPaginatedProducts.data[0]);
    });

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1'
    );
    expect(productRequest.request.method).toEqual('GET');

    productRequest.flush(testPaginatedProducts.data[0]);

    httpTestingController.verify();
  });

  it('should POST a product', () => {
    productDataService.createProduct(testProductFormValues).subscribe();

    const productRequest: TestRequest =
      httpTestingController.expectOne(baseUrl);
    expect(productRequest.request.method).toEqual('POST');

    productRequest.flush({});

    httpTestingController.verify();
  });

  it('should PUT a product', () => {
    productDataService
      .updateProduct(testProductFormValues, 1)
      .subscribe((updatedProduct: IProduct) => {
        expect(updatedProduct).toBeTruthy();
        expect(updatedProduct.name).toBe(testProductFormValues.name);
      });

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1'
    );
    expect(productRequest.request.method).toEqual('PUT');

    productRequest.flush(testProductFormValues);

    httpTestingController.verify();
  });

  it('should DELETE a product', () => {
    productDataService.deleteProduct(1).subscribe();

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1'
    );
    expect(productRequest.request.method).toEqual('DELETE');

    productRequest.flush({});

    httpTestingController.verify();
  });

  it('should DELETE a product photo', () => {
    const productId = 1;
    const photoId = 1;

    productDataService.deleteProductPhoto(productId, photoId).subscribe();

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + `${productId}/photo/${photoId}`
    );
    expect(productRequest.request.method).toEqual('DELETE');

    productRequest.flush({});

    httpTestingController.verify();
  });

  it('should PUT a product and add a review', () => {
    productDataService
      .addReviewToProduct(testReview)
      .subscribe((product: IProduct) => {
        expect(product).toBeTruthy();
        expect(product.reviews.length).toBe(1);
        expect(product.reviews[0]).toBe(testReview);
      });

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1/rating'
    );
    expect(productRequest.request.method).toEqual('PUT');

    productRequest.flush(testProductWithTestReview);

    httpTestingController.verify();
  });

  it('should PUT a product and set a new main photo', () => {
    const productId = 1;
    const photoId = 2;

    productDataService
      .setMainPhoto(photoId, productId)
      .subscribe((product: IProduct) => {
        expect(product).toBeTruthy();
        expect(product.photos[0].isMain).toBe(false);
        expect(product.photos[1].isMain).toBe(true);
      });

    const productRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + `${productId}/photo/${photoId}`
    );
    expect(productRequest.request.method).toEqual('PUT');

    const productToGetNewMainPhoto = testPaginatedProducts.data[0];
    productToGetNewMainPhoto.photos[0].isMain = false;
    productToGetNewMainPhoto.photos[1].isMain = true;

    productRequest.flush(productToGetNewMainPhoto);

    httpTestingController.verify();
  });
});
