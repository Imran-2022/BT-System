import { Routes } from '@angular/router';
// CRITICAL: Import your component using its correct relative path
import { SearchFormComponent } from './features/search/components/search-form/search-form'; 
import { BusDetails } from './bus-details/bus-details';

export const routes: Routes = [
    {
        path: '', 
        // Redirect the root path to the 'search' path
        redirectTo: 'search', 
        pathMatch: 'full' 
    },
    {
        path: 'search',
        // Load the SearchFormComponent when the path is /search
        component: SearchFormComponent
    },
    {
        path: 'bus/:id', 
        component: BusDetails
    }
];