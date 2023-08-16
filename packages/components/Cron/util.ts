import { Options, PeriodType, Unit } from "./types";

export const parseNumber = (value: unknown) => {
  if (typeof value === "string") {
    const str: string = value.trim();
    if (/^\d+$/.test(str)) {
      const num = Number(str);
      if (!isNaN(num) && isFinite(num)) {
        return num;
      }
    }
  } else if (typeof value === "number") {
    if (!isNaN(value) && isFinite(value) && value === Math.floor(value)) {
      return value;
    }
  }
  return undefined;
};

export const assertValidArray = (arr: unknown) => {
  if (
    arr === undefined ||
    !Array.isArray(arr) ||
    arr.length !== 5 ||
    arr.some((element) => !Array.isArray(element))
  ) {
    throw new Error("Invalid cron array");
  }
};

export const range = (start: number, end: number): number[] => {
  const array: number[] = [];
  for (let i = start; i <= end; i++) {
    array.push(i);
  }
  return array;
};

export const sort = (array: number[]) => [...array].sort((a, b) => a - b);

export const flatten = (arrays: number[][]) =>
  ([] as number[]).concat.apply([], arrays);

export const dedup = (array: number[]) => {
  const result: number[] = [];
  array.forEach((i) => {
    if (result.indexOf(i) < 0) {
      result.push(i);
    }
  });
  return result;
};

export const getPeriodFromCronParts = (cronParts: number[][]): PeriodType => {
  if (cronParts[3].length > 0) {
    return "Year";
  } else if (cronParts[2].length > 0) {
    return "Month";
  } else if (cronParts[4].length > 0) {
    return "Week";
  } else if (cronParts[1].length > 0) {
    return "Day";
  } else if (cronParts[0].length > 0) {
    return "Hour";
  }
  // return "minute";
  return "Hour";
};

export const fixFormatValue = (value: number) => {
  let result = value.toString();

  if (value < 10) {
    result = result.padStart(2, "0");
  }

  return result;
};

export const arrayToStringPart = (
  arr: number[],
  unit: Unit,
  options: Options
) => {
  const values = sort(
    dedup(
      fixSunday(
        arr.map((value) => {
          const parsedValue = parseNumber(value);
          if (parsedValue === undefined) {
            throw getError(`Invalid value "${value}"`, unit);
          }
          return parsedValue;
        }),
        unit
      )
    )
  );
  assertInRange(values, unit);
  return toString(values, unit, options);
};

export const stringToArrayPart = (str: string, unit: Unit) => {
  if (str === "*" || str === "*/1") {
    return [];
  }

  const values = sort(
    dedup(
      fixSunday(
        flatten(
          replaceAlternatives(str, unit)
            .split(",")
            .map((value: string) => {
              const valueParts = value.split("/");
              if (valueParts.length > 2) {
                throw getError(`Invalid value "${str}"`, unit);
              }
              let parsedValues: number[];
              const left = valueParts[0];
              const right = valueParts[1];
              if (left === "*") {
                parsedValues = range(unit.min, unit.max);
              } else {
                parsedValues = parseRange(left, str, unit);
              }
              const step = parseStep(right, unit);
              return applyInterval(parsedValues, step);
            })
        ),
        unit
      )
    )
  );
  assertInRange(values, unit);
  return values;
};

const toRanges = (values: number[]) => {
  const retval: number[][] = [];
  let startPart: number | undefined = undefined;
  values.forEach(function (value, index, self) {
    if (value !== self[index + 1] - 1) {
      if (startPart !== undefined) {
        retval.push([startPart, value]);
        startPart = undefined;
      } else {
        retval.push([value]);
      }
    } else if (startPart === undefined) {
      startPart = value;
    }
  });
  return retval;
};

const toString = (values: number[], unit: Unit, options: Options) => {
  let retval = "";
  if (isFull(values, unit) || values.length === 0) {
    if (options.outputHashes) {
      retval = "H";
    } else {
      retval = "*";
    }
  } else {
    const step = getStep(values);
    if (step && isInterval(values, step)) {
      if (isFullInterval(values, unit, step)) {
        if (options.outputHashes) {
          retval = `H/${step}`;
        } else {
          retval = `*/${step}`;
        }
      } else {
        const min = values[0];
        const max = values[values.length - 1];
        const range =
          formatValue(min, unit, options) +
          "-" +
          formatValue(max, unit, options);
        if (options.outputHashes) {
          retval = `H(${range})/${step}`;
        } else {
          retval = `${range}/${step}`;
        }
      }
    } else {
      retval = toRanges(values)
        .map((range) => {
          if (range.length === 1) {
            return formatValue(range[0], unit, options);
          } else {
            return (
              formatValue(range[0], unit, options) +
              "-" +
              formatValue(range[1], unit, options)
            );
          }
        })
        .join(",");
    }
  }

  return retval;
};

const formatValue = (value: number, unit: Unit, options: Options) => {
  if (
    (options.outputWeekdayNames && unit.name === "weekday") ||
    (options.outputMonthNames && unit.name === "month")
  ) {
    if (unit.alt) {
      return unit.alt[value - unit.min];
    }
  }
  return value;
};

const getError = (error: string, unit: Unit) =>
  new Error(`${error} for ${unit.name}`);

const parseRange = (rangeString: string, context: string, unit: Unit) => {
  const subparts = rangeString.split("-");
  if (subparts.length === 1) {
    const value = parseNumber(subparts[0]);
    if (value === undefined) {
      throw getError(`Invalid value "${context}"`, unit);
    }
    return [value];
  } else if (subparts.length === 2) {
    const minValue = parseNumber(subparts[0]);
    const maxValue = parseNumber(subparts[1]);
    if (minValue === undefined || maxValue === undefined) {
      throw getError(`Invalid value "${context}"`, unit);
    }
    if (maxValue < minValue) {
      throw getError(
        `Max range is less than min range in "${rangeString}"`,
        unit
      );
    }
    return range(minValue, maxValue);
  } else {
    throw getError(`Invalid value "${rangeString}"`, unit);
  }
};

const parseStep = (step: string, unit: Unit) => {
  if (step !== undefined) {
    const parsedStep = parseNumber(step);
    if (parsedStep === undefined) {
      throw getError(`Invalid interval step value "${step}"`, unit);
    }
    return parsedStep;
  }
  return 0;
};

const applyInterval = (values: number[], step: number) => {
  if (step) {
    const minVal = values[0];
    values = values.filter(
      (value) => value % step === minVal % step || value === minVal
    );
  }
  return values;
};

const fixSunday = (values: number[], unit: Unit) => {
  if (unit.name === "weekday") {
    values = values.map((value) => {
      if (value === 7) {
        return 0;
      }
      return value;
    });
  }
  return values;
};

const replaceAlternatives = (str: string, unit: Unit) => {
  if (unit.alt) {
    str = str.toUpperCase();
    for (let i = 0; i < unit.alt.length; i++) {
      str = str.replace(unit.alt[i], String(i + unit.min));
    }
  }
  return str;
};

const assertInRange = (values: number[], unit: Unit) => {
  const first = values[0];
  const last = values[values.length - 1];
  if (first < unit.min) {
    throw getError(`Value "${first}" out of range`, unit);
  } else if (last > unit.max) {
    throw getError(`Value "${last}" out of range`, unit);
  }
};

const isInterval = (values: number[], step: number) => {
  for (let i = 1; i < values.length; i++) {
    const prev = values[i - 1];
    const value = values[i];
    if (value - prev !== step) {
      return false;
    }
  }
  return true;
};

const isFullInterval = (values: number[], unit: Unit, step: number) => {
  const min = values[0];
  const max = values[values.length - 1];
  const haveAllValues = values.length === (max - min) / step + 1;
  if (min === unit.min && max + step > unit.max && haveAllValues) {
    return true;
  }
  return false;
};

const getStep = (values: number[]) => {
  if (values.length > 2) {
    const step = values[1] - values[0];
    if (step > 1) {
      return step;
    }
  }
  return 0;
};

const isFull = (values: number[], unit: Unit) => {
  return values.length === unit.max - unit.min + 1;
};
