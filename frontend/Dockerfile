FROM nginx:alpine

COPY ./frontend/doc-brasil/dist/doc-brasil/browser/ /usr/share/nginx/html

COPY nginx/conf.d/http.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

