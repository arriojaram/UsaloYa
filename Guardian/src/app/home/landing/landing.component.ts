import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserStateService } from '../../services/user-state.service';

declare const bootstrap: any;

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, AfterViewInit, OnDestroy {
  loading = true;

  private observers: IntersectionObserver[] = [];
  private scrollHandler: (() => void) | null = null;

  constructor(private userState: UserStateService) {}

  ngOnInit(): void {
    this.initializeLanding();
  }

  ngAfterViewInit(): void {
    document.body.classList.remove('sb-nav-fixed');
    this.initNavbarScroll();
    this.initRevealOnScroll();
    this.initStatsCounter();
    this.initSmoothScroll();
  }

  ngOnDestroy(): void {
    this.observers.forEach(o => o.disconnect());
    if (this.scrollHandler) {
      window.removeEventListener('scroll', this.scrollHandler);
    }
    document.body.classList.add('sb-nav-fixed');
  }

  private initializeLanding(): void {
    this.loading = false;
    const userStatus = this.userState.getUserStateLocalStorage();
    if (userStatus) {
      userStatus.notInHomePage = false;
      this.userState.setUserStateLocalStorage(userStatus);
    }
  }

  private initNavbarScroll(): void {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;
    this.scrollHandler = () => {
      navbar.classList.toggle('scrolled', window.scrollY > 50);
    };
    window.addEventListener('scroll', this.scrollHandler, { passive: true });
  }

  private initRevealOnScroll(): void {
    const isMobile = window.innerWidth < 768;
    const revealEls = document.querySelectorAll<HTMLElement>('.reveal-on-scroll');

    if (isMobile) {
      return;
    }

    revealEls.forEach(el => {
      el.style.opacity = '0';
      el.style.transform = 'translateY(28px)';
      const delay = el.style.transitionDelay;
      el.style.transition = delay
        ? `opacity 0.65s ease ${delay}, transform 0.65s ease ${delay}`
        : 'opacity 0.65s ease, transform 0.65s ease';
    });

    const revealObserver = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          (entry.target as HTMLElement).style.opacity = '1';
          (entry.target as HTMLElement).style.transform = 'translateY(0)';
          revealObserver.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

    revealEls.forEach(el => revealObserver.observe(el));
    this.observers.push(revealObserver);
  }

  private initStatsCounter(): void {
    const statNumbers = document.querySelectorAll<HTMLElement>('.stat-number');
    const counterObserver = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        const el = entry.target as HTMLElement;
        const target = parseInt(el.getAttribute('data-target') ?? '0', 10);
        const duration = 1400;
        const startTime = performance.now();
        const tick = (now: number) => {
          const elapsed = now - startTime;
          const progress = Math.min(elapsed / duration, 1);
          const eased = 1 - Math.pow(1 - progress, 3);
          el.textContent = String(Math.round(eased * target));
          if (progress < 1) requestAnimationFrame(tick);
        };
        requestAnimationFrame(tick);
        counterObserver.unobserve(el);
      });
    }, { threshold: 0.5 });

    statNumbers.forEach(el => counterObserver.observe(el));
    this.observers.push(counterObserver);
  }

  private initSmoothScroll(): void {
    document.querySelectorAll<HTMLAnchorElement>('a[href^="#"]').forEach(anchor => {
      anchor.addEventListener('click', (e: MouseEvent) => {
        const href = (e.currentTarget as HTMLAnchorElement).getAttribute('href');
        if (!href) return;
        const target = document.querySelector<HTMLElement>(href);
        if (!target) return;
        e.preventDefault();
        const top = target.getBoundingClientRect().top + window.scrollY - 80;
        window.scrollTo({ top, behavior: 'smooth' });
        const navMenu = document.getElementById('navMenu');
        if (navMenu && navMenu.classList.contains('show')) {
          bootstrap.Collapse.getOrCreateInstance(navMenu).hide();
        }
      });
    });
  }
}
