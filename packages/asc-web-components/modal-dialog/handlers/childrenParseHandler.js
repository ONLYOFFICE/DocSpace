export const parseChildren = (
  children,
  headerDisplayName,
  bodyDisplayName,
  footerDisplayName
) => {
  let header = null,
    body = null,
    footer = null;

  children.forEach((child) => {
    const childType =
      child && child.type && (child.type.displayName || child.type.name);

    switch (childType) {
      case headerDisplayName:
        header = child;
        break;
      case bodyDisplayName:
        body = child;
        break;
      case footerDisplayName:
        footer = child;
        break;
      default:
        break;
    }
  });
  return [header, body, footer];
};
