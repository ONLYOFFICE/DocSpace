import React, { useEffect, useState } from "react";
import type { Meta, StoryObj } from "@storybook/react";

import Cron from ".";

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

    return (
      <div>
        <input
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
      </div>
    );
  },
};
