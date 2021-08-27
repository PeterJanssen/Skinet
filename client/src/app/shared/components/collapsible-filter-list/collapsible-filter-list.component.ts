import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-collapsible-filter-list',
  templateUrl: './collapsible-filter-list.component.html',
  styleUrls: ['./collapsible-filter-list.component.scss'],
})
export class CollapsibleFilterListComponent implements OnInit {
  @Input() title: string;
  @Input() filters: any;
  @Input() id: number;
  @Output() newFilterIdSelected = new EventEmitter<number>();
  isCollapsed = false;

  constructor() {}

  ngOnInit(): void {}

  onFilterSelected(filterId: number) {
    this.newFilterIdSelected.emit(filterId);
  }
}
