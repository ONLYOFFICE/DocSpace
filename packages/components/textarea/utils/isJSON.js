export function isJSON(jsonString) {
  try {
    const parsedJson = JSON.parse(jsonString);
    return parsedJson && typeof parsedJson === "object";
  } catch (e) {}

  return false;
}