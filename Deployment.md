# Deployment Guide

This project can be deployed using a simple free-friendly stack:
- Backend: Railway
- Frontend: Netlify
- Database: Neon (PostgreSQL)

## 1. Prepare the database on Neon
1. Create a free database on Neon.
2. Copy the PostgreSQL connection string.
3. Keep it safe because you will use it in Railway.

## 2. Deploy the backend to Railway
1. Push the project to GitHub.
2. Open Railway and create a new project.
3. Connect your GitHub repository.
4. Select the backend project or configure the service to run the .NET app.
5. Set these environment variables in Railway:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__DefaultConnection=<your-neon-connection-string>`
   - `Cors__AllowedOrigins=https://your-netlify-app.netlify.app`
6. Deploy the service.

## 3. Update the frontend API URL
In the Angular frontend, update the production API base URL in `src/ClientApp/src/environments/environment.prod.ts` to your Railway backend URL.

Example:
```ts
export const environment = {
  production: true,
  apiUrl: 'https://your-railway-app.up.railway.app/api'
};
```

## 4. Deploy the frontend to Netlify
1. Connect the same GitHub repository to Netlify.
2. Set the build command:
   ```bash
   npm install && npm run build
   ```
3. Set the publish directory to the Angular build output (usually `dist/client-app`).
4. Add the SPA redirect file at `src/ClientApp/public/_redirects` with this content:
   ```txt
   /* /index.html 200
   ```
5. Deploy the site.

## 5. Test the live application
After deployment, verify the following:
- the frontend loads correctly on Netlify
- the API is reachable from Railway
- the database connection works
- booking and search operations work end to end

## 6. Important production notes
- Do not use `localhost` in the live frontend configuration.
- Use environment variables for the production database connection.
- Make sure the backend allows requests from your Netlify frontend domain via CORS.
