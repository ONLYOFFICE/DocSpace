//  Maximum allowed email length
// https://www.lifewire.com/is-email-address-length-limited-1171110
export const MAX_EMAIL_LENGTH = 323;

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
  let isSimple = true;
  let tempLabel = "";

  const stringChips = value
    .split(new RegExp(separators.join("|"), "g"))
    .filter((it) => it.trim().length !== 0)
    .map((it, idx, arr) => {
      if (
        it.startsWith('"') &&
        it.endsWith('"') &&
        arr[idx + 1]?.startsWith("<")
      ) {
        indexesForFilter.push(idx + 1);
        return arr[idx + 1] ? `${it} ${arr[idx + 1]}` : it;
      } else if (
        it.startsWith('"') &&
        it.endsWith('"') &&
        !arr[idx + 1]?.startsWith("<")
      ) {
        return it;
      }
      if (isSimple && it.startsWith('"') && !arr[idx + 1]?.startsWith("<")) {
        isSimple = false;
        tempLabel += `${it} `;
        return;
      }
      if (!isSimple && !it.includes('"')) {
        tempLabel += `${it} `;
        return;
      }
      if (!isSimple && it.includes('"') && arr[idx + 1]?.startsWith("<")) {
        tempLabel += `${it}`;
        let tempLabelTrim = tempLabel;
        isSimple = true;
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
