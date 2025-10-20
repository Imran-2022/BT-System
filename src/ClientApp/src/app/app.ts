import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true, // Assuming standalone is default
  // Add RouterOutlet to imports so routing works
  imports: [RouterOutlet], 
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  // We don't need the title signal anymore, as content is in the router-outlet
}