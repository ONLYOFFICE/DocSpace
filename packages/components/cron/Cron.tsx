import { useTranslation } from "react-i18next";
import React, { useState, useEffect, useRef, useMemo } from "react";

import { MonthDays, Months, Period, WeekDays, Hours, Minutes } from "./Field";

import { getCronStringFromValues, stringToArray } from "./part";
import { defaultCronString, defaultPeriod } from "./constants";
import { getPeriodFromCronParts, getUnits } from "./util";

import { CronWrapper, Suffix } from "./Cron.styled";

import type CronProps from "./Cron.props";
import type { PeriodType } from "./types";

function Cron({ value = defaultCronString, setValue, onError }: CronProps) {
  const { t } = useTranslation("Common");

  const valueRef = useRef<string>(value);

  const [period, setPeriod] = useState<PeriodType>(defaultPeriod);

  const [hours, setHours] = useState<number[]>([]);
  const [months, setMonths] = useState<number[]>([]);
  const [minutes, setMinutes] = useState<number[]>([]);
  const [weekDays, setWeekDays] = useState<number[]>([]);
  const [monthDays, setMonthDays] = useState<number[]>([]);

  useEffect(() => {
    onError?.(undefined); // reset error state
    if (valueRef.current !== value) init();
  }, [value]);

  useEffect(() => {
    try {
      const cornString = getCronStringFromValues(
        period,
        months,
        monthDays,
        weekDays,
        hours,
        minutes
      );

      setValue(cornString);
      valueRef.current = cornString;

      onError?.(undefined);
    } catch (error) {
      if (error instanceof Error) onError?.(error);
    }
  }, [period, hours, months, minutes, weekDays, monthDays]);

  useEffect(() => {
    init();
  }, []);

  const init = () => {
    try {
      const cronParts = stringToArray(value);
      const period = getPeriodFromCronParts(cronParts);

      const [minutes, hours, monthDays, months, weekDays] = cronParts;

      setMinutes(minutes);
      setHours(hours);
      setMonthDays(monthDays);
      setMonths(months);
      setWeekDays(weekDays);

      setPeriod(period);
    } catch (error) {
      console.log(error);
      if (error instanceof Error) onError?.(error);
    }
  };

  const { isYear, isMonth, isWeek, isHour, isMinute } = useMemo(() => {
    const isYear = period === "Year";
    const isMonth = period === "Month";
    const isWeek = period === "Week";
    const isHour = period === "Hour";
    const isMinute = period == "Minute";

    return {
      isYear,
      isMonth,
      isWeek,
      isHour,
      isMinute,
    };
  }, [period]);

  const units = useMemo(() => getUnits(t), [t]);

  return (
    <CronWrapper>
      <Period t={t} period={period} setPeriod={setPeriod} />
      {isYear && (
        <Months unit={units[3]} t={t} months={months} setMonths={setMonths} />
      )}
      {(isYear || isMonth) && (
        <MonthDays
          t={t}
          unit={units[2]}
          weekDays={weekDays}
          monthDays={monthDays}
          setMonthDays={setMonthDays}
        />
      )}
      {(isYear || isMonth || isWeek) && (
        <WeekDays
          t={t}
          unit={units[4]}
          isWeek={isWeek}
          period={period}
          monthDays={monthDays}
          weekDays={weekDays}
          setWeekDays={setWeekDays}
        />
      )}
      {!isHour && !isMinute && (
        <Hours unit={units[1]} t={t} hours={hours} setHours={setHours} />
      )}

      {!isMinute && (
        <Minutes
          t={t}
          unit={units[0]}
          period={period}
          minutes={minutes}
          setMinutes={setMinutes}
        />
      )}
      <Suffix>{t("Common:UTC")}</Suffix>
    </CronWrapper>
  );
}

export default Cron;
