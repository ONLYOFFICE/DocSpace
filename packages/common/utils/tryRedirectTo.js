export default function (page) {
  if (
    window.location.pathname === page ||
    window.location.pathname.indexOf(page) !== -1
  ) {
    return false;
  }

  window.location.replace(page);

  return true;
}
