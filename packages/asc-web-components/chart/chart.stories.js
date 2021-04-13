import React, { useRef } from "react";

import Chart from "./";

export default {
  title: "Components/Chart",
  component: Chart,
  parameters: {
    docs: {
      description: {
        component: `Chart component`,
      },
    },
  },
};

const getRandData = (items) => {
  return items.map(() => Math.floor(Math.random() * (Math.floor(100) - 0)) + 0);
};

const Template = (args) => {
  const labels = Array.from({ length: 10 }, (v, i) => "Item " + i);
  const basicData = {
    labels: labels,
    datasets: [
      {
        label: "Visits",
        data: getRandData(labels),
      },
    ],
  };

  const basicOptions = {
    scales: {
      x: [
        {
          ticks: {
            fontColor: "#A3A3A3",
          },
        },
      ],
      y: [
        {
          ticks: {
            fontColor: "#A3A3A3",
          },
        },
      ],
    },
  };

  return (
    <Chart type="line" data={basicData} options={basicOptions} height="300px" />
  );
};

export const Default = Template.bind({});
