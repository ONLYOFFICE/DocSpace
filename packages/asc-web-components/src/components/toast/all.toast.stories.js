import React from "react";
import ReactDOM from "react-dom";
import { storiesOf } from "@storybook/react";
import Toast from ".";
import toastr from "./toastr";
import Link from "../link";

class TostWrapper extends React.Component {
  componentDidMount() {
    this.toastContainer = document.createElement("div");
    this.toastContainer.setAttribute("id", "toast-container");
    document.body.appendChild(this.toastContainer);

    ReactDOM.render(
      <Toast />,
      document.getElementById("toast-container"),
      () => {
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
        toastr.info(
          "Demo text for info manual closed Toast",
          null,
          0,
          true,
          true
        );
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
      }
    );
  }

  componentWillUnmount() {
    toastr.clear();
    ReactDOM.unmountComponentAtNode(this.toastContainer);
    document.body.removeChild(this.toastContainer);
  }

  render() {
    return <></>;
  }
}

storiesOf("Components|Toast", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => <TostWrapper />);
