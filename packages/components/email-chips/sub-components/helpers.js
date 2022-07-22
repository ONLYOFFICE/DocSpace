//  Maximum allowed email length
// https://www.lifewire.com/is-email-address-length-limited-1171110
export const MAX_EMAIL_LENGTH = 320;
export const MAX_EMAIL_LENGTH_WITH_DOTS = 323;
const MAX_DISPLAY_NAME_LENGTH = 64;
const MAX_VALUE_LENGTH = 256;

export const truncate = (str, length) =>
  str?.length > length ? str?.slice(0, length) + "..." : str;

export const sliceEmail = (it) => {
  if (typeof it === "string") {
    const res = truncate(it, MAX_EMAIL_LENGTH);
    return {
      name: res,
      email: res,
    };
  }
  return {
    ...it,
    name: truncate(it?.name, MAX_DISPLAY_NAME_LENGTH),
    email: truncate(it?.email, MAX_VALUE_LENGTH),
  };
};
