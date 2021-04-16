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

/* 
        backgroundColor: "#ffffff",
        titleColor: "#333333",
        titleFont: {
          family: "Open Sans, sans-serif, Arial",
          size: 11,
          lineHeight: 1.35,
        },
        bodyColor: "#A3A3A3",
        bodyFont: {
          family: "Open Sans, sans-serif, Arial",
          size: 11,
          lineHeight: 1.35,
        },
        padding: 16,
        displayColors: false,
        borderColor: "#ECEEF1",
        borderWidth: 1,
        caretSize: 5,
*/

const getRandData = (items) => {
  return items.map(() => Math.floor(Math.random() * (Math.floor(100) - 0)) + 0);
};

const Template = (args) => {
  const labels = Array.from({ length: 10 }, (v, i) => "Jun " + i);

  const data = {
    labels: labels,
    datasets: [
      {
        label: "Visits",
        data: getRandData(labels),
        tension: 0.4,
        pointStyle: "circle",
        pointRadius: 4,
      },
    ],
  };

  const options = {
    plugins: {
      legend: {
        display: false,
      },
      tooltip: {
        backgroundColor: "#ffffff",
        titleColor: "#333333",
        titleFont: {
          family: "Open Sans, sans-serif, Arial",
          size: 11,
          lineHeight: 1.35,
        },
        bodyColor: "#A3A3A3",
        bodyFont: {
          family: "Open Sans, sans-serif, Arial",
          size: 11,
          lineHeight: 1.35,
        },
        padding: 16,
        displayColors: false,
        borderColor: "#ECEEF1",
        borderWidth: 1,
        caretSize: 5,
        callbacks: {
          title: body,
          body: title,
        },
      },
    },
    interaction: {
      mode: "nearest",
    },
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
    <div style={{ height: "50vh" }}>
      <Chart type="line" data={data} options={options} />
    </div>
  );
};

export const Default = Template.bind({});
