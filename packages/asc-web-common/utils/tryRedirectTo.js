import history from "../history";
export default function (page) {
  if (
    window.location.pathname === page ||
    window.location.pathname.indexOf(page) !== -1
  ) {
    return false;
  }
  //TODO: check if we already on default page
  window.location.replace(page);
  history.push(page); // SSR crash

  return true;
}
