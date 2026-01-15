export async function apiFetch(input: RequestInfo, init: RequestInit = {}) {

    const accessToken = sessionStorage.getItem("accessToken");

    const doFetch = (token: string | null) => 
        fetch(input, {
            ...init,
            headers: {
                ...(init.headers || {}),
                ...(token ? { Authorization: `Bearer ${token}` } : {}),
            },
            credentials: "include",
        });

    let res = await doFetch(accessToken);

    if (res.status !== 401) {
        return res;
    }

    const refreshRes = await fetch("http://localhost:5162/auth/refresh", {
        method: "POST",
        credentials: "include",
    });

    if (!refreshRes.ok) {
        sessionStorage.removeItem("accessToken");
        window.location.href = "/login";
        return res;
    }

    const data = await refreshRes.json();
    sessionStorage.setItem("accessToken", data.accessToken);

    return doFetch(data.accessToken);


}
    
