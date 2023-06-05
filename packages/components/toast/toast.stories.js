import React, { useEffect } from "react";
import ReactDOM from "react-dom";
import Toast from "./";
import toastr from "./toastr";
import Button from "../button";
import Link from "../link";

export default {
  title: "Components/Toast",
  component: Toast,

  argTypes: {
    withCross: {
      description:
        "If `false`: toast disappeared after clicking on any area of toast. If `true`: toast disappeared after clicking on close button",
    },
    timeout: {
      description:
        "Time (in milliseconds) for showing your toast. Setting in `0` let you to show toast constantly until clicking on it",
    },
    data: {
      description: "Any components or data inside a toast",
    },
  },
};

const BaseTemplate = ({ type, data, title, timeout, withCross, ...args }) => {
  return (
    <>
      <Toast {...args} />
      <Button
        label="Show toast"
        primary
        size="small"
        onClick={() => {
          switch (type) {
            case "error":
              toastr.error(data, title, timeout, withCross);
              break;
            case "warning":
              toastr.warning(data, title, timeout, withCross);
              break;
            case "info":
              toastr.info(data, title, timeout, withCross);
              break;
            default:
              toastr.success(data, title, timeout, withCross);
              break;
          }
        }}
      />
    </>
  );
};

export const basic = BaseTemplate.bind({});
basic.args = {
  type: "success",
  data: "Demo text for Toast",
  title: "Demo title",
  withCross: false,
  timeout: 5000,
};

const AllTemplate = (args) => {
  const renderAllToast = () => {
    toastr.success(
      "Demo text for success Toast closes in 30 seconds or on click",
      null,
      30000
    );
    toastr.error(
      "Demo text for error Toast closes in 28 seconds or on click",
      null,
      28000
    );
    toastr.warning(
      "Demo text for warning Toast closes in 25 seconds or on click",
      null,
      25000
    );
    toastr.info(
      "Demo text for info Toast closes in 15 seconds or on click",
      null,
      15000
    );
    toastr.success(
      "Demo text for success Toast with title closes in 12 seconds or on click",
      "Demo title",
      12000
    );
    toastr.error(
      "Demo text for error Toast with title closes in 10 seconds or on click",
      "Demo title",
      10000
    );
    toastr.warning(
      "Demo text for warning Toast with title closes in 8 seconds or on click",
      "Demo title",
      8000
    );
    toastr.info(
      "Demo text for info Toast with title closes in 6 seconds or on click",
      "Demo title",
      6000
    );
    toastr.success(
      "Demo text for success manual closed Toast",
      null,
      0,
      true,
      true
    );
    toastr.error(
      "Demo text for error manual closed Toast",
      null,
      0,
      true,
      true
    );
    toastr.warning(
      "Demo text for warning manual closed Toast",
      null,
      0,
      true,
      true
    );
    toastr.info("Demo text for info manual closed Toast", null, 0, true, true);
    toastr.success(
      <>
        Demo text for success manual closed Toast with title and contains{" "}
        <Link
          type="page"
          color="gray"
          href="https://github.com"
          text="gray link"
        />
      </>,
      "Demo title",
      0,
      true,
      true
    );
    toastr.error(
      "Demo text for error manual closed Toast with title",
      "Demo title",
      0,
      true,
      true
    );
    toastr.warning(
      "Demo text for warning manual closed Toast with title",
      "Demo title",
      0,
      true,
      true
    );
    toastr.info(
      "Demo text for info manual closed Toast with title",
      "Demo title",
      0,
      true,
      true
    );
  };
  return (
    <>
      <Toast />
      <Button
        label="Show all toast"
        primary
        size="small"
        onClick={() => renderAllToast()}
      />
    </>
  );
};

export const all = AllTemplate.bind({});
