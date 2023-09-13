import React, { useEffect, useMemo, useState } from "react";
import type { Meta, StoryObj } from "@storybook/react";

import Cron, { getNextSynchronization } from ".";
import TextInput from "../text-input/text-input";
import Button from "../button";

type CronType = typeof Cron;

type Story = StoryObj<CronType>;

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
  },
};

export default meta;

export const Default: Story = {
  args: {},

  render: ({ value: defaultValue }, context) => {
    const [input, setInput] = useState(defaultValue);

    const [cron, setCron] = useState(defaultValue);
    const [error, setError] = useState<Error>();

    const { locale } = context.globals;

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
