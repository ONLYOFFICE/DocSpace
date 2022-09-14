export const getConvertedSize = (t, size) => {
  let sizeNames;

  if (size < 1024 * 1024) {
    sizeNames = [t("Megabyte"), t("Gigabyte"), t("Terabyte")];
  } else {
    sizeNames = [
      t("Bytes"),
      t("Kilobyte"),
      t("Megabyte"),
      t("Gigabyte"),
      t("Terabyte"),
    ];
  }

  const bytes = size;

  if (bytes == 0) return `${"0" + " " + t("Bytes")}`;

  const i = Math.floor(Math.log(bytes) / Math.log(1024));

  return (
    parseFloat((bytes / Math.pow(1024, i)).toFixed(2)) + " " + sizeNames[i]
  );
};
