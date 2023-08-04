import json_beautifier from "csvjson-json_beautifier";

export function beautifyJSON(jsonString) {
  return json_beautifier(JSON.parse(jsonString), {
    inlineShortArrays: true,
  });
}
