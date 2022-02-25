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
  if (email[email.length - 1] != ">") return false;
  email = email.slice(0, -1);

  return { label, value: email };
};
