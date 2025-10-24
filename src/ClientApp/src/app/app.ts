import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true, 
  // RouterOutlet to imports so routing works
  imports: [RouterOutlet], 
  templateUrl: './app.html',
})
export class App {}