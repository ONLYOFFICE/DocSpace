import React, { FC, useEffect, useMemo, useState } from "react";
import type { Meta, StoryObj } from "@storybook/react";
import { useTranslation } from "react-i18next";
import i18nextStoryDecorator from "../.storybook/decorators/i18nextStoryDecorator";

import Cron, { getNextSynchronization } from ".";
import TextInput from "../text-input/text-input";
import Button from "../button";
import CronProps from "./Cron.props";

type CronType = FC<{ locale: string } & CronProps>;

type Story = StoryObj<CronType>;

const locales = [
  "az",
  "ar-SA",
  "zh-cn",
  "cs",
  "nl",
  "en",
  "fi",
  "fr",
  "de",
  "de-ch",
  "el",
  "it",
  "ja",
  "ko",
  "lv",
  "pl",
  "pt",
  "pt-br",
  "ru",
  "sk",
  "sl",
  "es",
  "tr",
  "uk",
  "vi",
];

const meta: Meta<CronType> = {
  title: "Components/Cron",
  component: Cron,
  argTypes: {
    value: {
      description: "Cron value",
    },
    setValue: {
      description: "Set the cron value, similar to onChange.",
    },
    onError: {
      description:
        "Triggered when the cron component detects an error with the value.",
    },
    locale: { control: "select", options: locales },
  },
  decorators: [i18nextStoryDecorator],
};

export default meta;

export const Default: Story = {
  args: {
    locale: "en",
  },

  render: ({ value: defaultValue, locale }) => {
    const { i18n } = useTranslation();

    const [input, setInput] = useState(defaultValue);

    const [cron, setCron] = useState(defaultValue);
    const [error, setError] = useState<Error>();

    useEffect(() => {
      i18n.changeLanguage(locale);
    }, [locale]);

    const onError = (error?: Error) => {
      setError(error);
    };

    const setValue = (cron?: string) => {
      setInput(cron);
      setCron(cron);
    };

    const onClick = () => {
      setCron(input);
    };

    useEffect(() => {
      setValue(defaultValue);
    }, [defaultValue]);

    const date = useMemo(() => cron && getNextSynchronization(cron), [cron]);

    return (
      <div>
        <div
          style={{
            display: "flex",
            gap: "6px",
            alignItems: "baseline",
            maxWidth: "max-content",
            marginBottom: "8px",
          }}
        >
          <TextInput
            value={input}
            onChange={(e) => setInput(e.target.value)}
            hasError={!!error}
            scale={false}
          />
          {/*@ts-ignore*/}
          <Button size="small" primary label={"Set value"} onClick={onClick} />
        </div>

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
            {date
              .toUTC()
              .setLocale(locale ?? "en")
              .toFormat("DDDD tt")}
          </p>
        )}
      </div>
    );
  },
};
