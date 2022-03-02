//  Maximum allowed email length
// https://www.lifewire.com/is-email-address-length-limited-1171110
export const MAX_EMAIL_LENGTH = 320;
export const MAX_EMAIL_LENGTH_WITH_DOTS = 323;
const MAX_LABEL_LENGTH = 64;
const MAX_VALUE_LENGTH = 256;

export const tryParseEmail = (emailString) => {
  const cortege = emailString
    .trim()
    .split('" <')
    .map((it) => it.trim());

  if (cortege.length != 2) return false;
  let label = cortege[0];
  if (label[0] != '"') return false;
  label = label.slice(1);

  let email = cortege[1];
  if (email === undefined) return false;
  if (email[email.length - 1] != ">") return false;
  email = email.slice(0, -1);

  return { label, value: email };
};

export const getChipsFromString = (value) => {
  const separators = [",", " ", ", "];
  let indexesForFilter = [];
  let isOneWordLabel = true;
  let tempLabel = "";

  const stringChips = value
    .split(new RegExp(separators.join("|"), "g"))
    .filter((it) => it.trim().length !== 0)
    .map((it, idx, arr) => {
      const isStartWithQuote = it.startsWith('"');
      const isEndwithQuote = it.endsWith('"');
      const isNextStartWithAngle = arr[idx + 1]?.startsWith("<");

      if (isStartWithQuote && isEndwithQuote) {
        if (isNextStartWithAngle) {
          indexesForFilter.push(idx + 1);
          return arr[idx + 1] ? `${it} ${arr[idx + 1]}` : it;
        } else {
          return it;
        }
      }
      if (isOneWordLabel && isStartWithQuote && !isNextStartWithAngle) {
        isOneWordLabel = false;
        tempLabel += `${it} `;
        return;
      }
      if (!isOneWordLabel && !it.includes('"')) {
        tempLabel += `${it} `;
        return;
      }
      if (!isOneWordLabel && isEndwithQuote && isNextStartWithAngle) {
        tempLabel += `${it}`;
        let tempLabelTrim = tempLabel;
        isOneWordLabel = true;
        tempLabel = "";
        indexesForFilter.push(idx + 1);
        return arr[idx + 1]
          ? `${tempLabelTrim} ${arr[idx + 1]}`
          : tempLabelTrim;
      }
      return it;
    })
    .filter((it, idx) => !indexesForFilter.includes(idx) && it !== undefined);
  return Array.from(new Set(stringChips)).map((it) =>
    tryParseEmail(it) ? tryParseEmail(it) : it.trim()
  );
};

export const truncate = (str) =>
  str?.length > MAX_EMAIL_LENGTH
    ? str?.slice(0, MAX_EMAIL_LENGTH) + "..."
    : str;

export const sliceEmail = (it) => {
  if (typeof it === "string") {
    const res = truncate(it);
    return {
      label: res,
      value: res,
    };
  }
  return {
    label: truncate(it?.label),
    value: truncate(it?.value),
  };
};
