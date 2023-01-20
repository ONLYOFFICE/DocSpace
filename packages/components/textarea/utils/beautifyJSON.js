import json_beautifier from "csvjson-json_beautifier";

export function beautifyJSON(jsonString) {
  const beautifiedValue = json_beautifier(JSON.parse(jsonString), {
    inlineShortArrays: true,
  });

  return beautifiedValue
    .split("\n")
    .map((value, index) => index + 1 + `\t` + value)
    .join("\n");
}
