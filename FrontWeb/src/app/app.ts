import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from './core/layout/footer/footer';
import { Header } from './core/layout/header/header';

@Component({
  imports: [Footer, Header, RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {}
