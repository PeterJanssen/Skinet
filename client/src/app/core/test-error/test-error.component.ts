import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss'],
})
export class TestErrorComponent implements OnInit {
  validationErrors: any;
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {}

  get404Error(): void {
    this.http.get(this.baseUrl + 'products/-1').subscribe(
      (response) => console.log(response),
      (error) => console.log(error)
    );
  }

  get500Error(): void {
    this.http.get(this.baseUrl + 'buggy/servererror').subscribe(
      (response) => console.log(response),
      (error) => console.log(error)
    );
  }

  get400Error(): void {
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe(
      (response) => console.log(response),
      (error) => console.log(error)
    );
  }

  get400ValidationError(): void {
    this.http.get(this.baseUrl + 'products/fortytwo').subscribe(
      (response) => console.log(response),
      (error) => {
        console.log(error);
        this.validationErrors = error.errors;
      }
    );
  }
}
