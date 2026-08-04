# Deployment Guide

This project can be deployed using a free-friendly stack:
- Backend: Render
- Frontend: Netlify
- Database: Neon (PostgreSQL)

## 1. Prepare the database on Neon
1. Create a free database on Neon.
2. Copy the PostgreSQL connection string.
3. Keep it safe because you will use it in Render.

## 2. Deploy the backend to Render
1. Push the project to GitHub.
2. Open Render and create a new Web Service.
3. Connect your GitHub repository.
4. Use the repository root, then set the service to use the `src` folder as the deploy root if prompted.
5. Set the Dockerfile path to `src/WebApi/Dockerfile`.
6. Set these environment variables in Render:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__DefaultConnection=<your-neon-connection-string>`
   - `Cors__AllowedOrigins=https://your-netlify-app.netlify.app`
7. Deploy the service.

## 3. Update the frontend API URL
In the Angular frontend, update the production API base URL in `src/ClientApp/src/environments/environment.prod.ts` to your Render backend URL.

Example:
```ts
export const environment = {
  production: true,
  apiUrl: 'https://your-render-service.onrender.com/api'
};
```

## 4. Deploy the frontend to Netlify
1. Connect the same GitHub repository to Netlify.
2. Set the build command:
   ```bash
   npm install && npm run build
   ```
3. Set the publish directory to the Angular build output, for example `dist/client-app`.
4. Add the SPA redirect file at `src/ClientApp/public/_redirects` with this content:
   ```txt
   /* /index.html 200
   ```
5. Deploy the site.

## 5. Test the live application
After deployment, verify the following:
- the frontend loads correctly on Netlify
- the API is reachable from Render
- the database connection works
- booking and search operations work end to end

## 6. Important production notes
- Do not use `localhost` in the live frontend configuration.
- Use environment variables for the production database connection.
- Make sure the backend allows requests from your Netlify frontend domain via CORS.
- Set Render to use the `src/WebApi/Dockerfile` and the `src` context root so build paths match.
