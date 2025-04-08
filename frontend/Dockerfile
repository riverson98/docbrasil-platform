FROM nginx:alpine

# Copia os arquivos estáticos do Angular (já buildados)
COPY doc-brasil/dist/doc-brasil/browser/ /usr/share/nginx/html

# Copia a configuração customizada do nginx
COPY nginx/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
