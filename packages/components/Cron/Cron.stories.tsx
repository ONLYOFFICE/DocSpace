import React, { useEffect, useMemo, useState } from "react";
import { initReactI18next } from "react-i18next";
import i18n from "i18next";

import Cron, { getNextSynchronization } from ".";

import type { Meta, StoryObj } from "@storybook/react";

i18n.use(initReactI18next).init({
  resources: {
    en: {
      Cron: {
        EveryYear: "Every year",
        EveryMonth: "Every month",
        EveryWeek: "Every week",
        EveryHour: "Every hour",
        EveryMinute: "Every minute",
        In: "in",
        At: "at",
        On: "on",
        And: "and",
        EveryDayOfTheMonth: "Every day of the month",
        DayOfTheMonth: "Day of the month",
        EveryDayOfTheWeek: "Every day of the week",
        DayOfTheWeek: "Day of the week",

        JAN: "JAN",
        FEB: "FEB",
        MAR: "MAR",
        APR: "APR",
        MAY: "MAY",
        JUN: "JUN",
        JUL: "JUL",
        AUG: "AUG",
        SEP: "SEP",
        OCT: "OCT",
        NOV: "NOV",
        DEC: "DEC",

        SUN: "SUN",
        MON: "MON",
        TUE: "TUE",
        WED: "WED",
        THU: "THU",
        FRI: "FRI",
        SAT: "SAT",
      },
    },
  },

  lng: "en",
  fallbackLng: "en",
  interpolation: {
    escapeValue: false,
  },
});

type CronType = typeof Cron;

type Story = StoryObj<CronType>;

const meta: Meta<CronType> = {
  title: "Components/Cron",
  component: Cron,
};

export default meta;

export const Default: Story = {
  args: {},

  render: ({ value: defaultValue }) => {
    const [input, setInput] = useState(defaultValue);

    const [cron, setCron] = useState(defaultValue);
    const [error, setError] = useState<Error>();

    const onError = (error?: Error) => {
      setError(error);
    };

    const setValue = (cron?: string) => {
      setInput(cron);
      setCron(cron);
    };

    useEffect(() => {
      setValue(defaultValue);
    }, [defaultValue]);

    const date = useMemo(() => cron && getNextSynchronization(cron), [cron]);

    return (
      <div>
        <input
          style={{
            borderColor: error ? "red" : "black",
          }}
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onBlur={(e) => setCron(e.target.value)}
        />
        <Cron value={cron} setValue={setValue} onError={onError} />
        <p>
          <strong>Cron string: </strong> {cron}
        </p>
        <p>
          <strong>Error message: </strong> {error?.message ?? "undefined"}
        </p>
        {date && (
          <p>
            <strong>Next synchronization: </strong>{" "}
            {date.toUTC().setLocale("en-GB").toFormat("DDDD tt")}
          </p>
        )}
      </div>
    );
  },
};
