import React from "react";
import { storiesOf } from "@storybook/react";
import Button from ".";
import Section from "../../../.storybook/decorators/section";

function onClick(e) {
  e = e || window.event;
  var target = e.target || e.srcElement,
    text = target.textContent || target.innerText;
  console.log("onClick", text);
}

const getButtons = (primary) => {
  const sizes = ["large", "big", "medium", "base"];
  const states = ["isActivated", "isHovered", "isClicked", "isDisabled"];

  const baseButton = {
    size: "base",
    primary: true,
    isActivated: false,
    isHovered: false,
    isClicked: false,
    isDisabled: false,
    isLoading: false,
    onClick: onClick,
    label: "base button",
  };

  let buttons = [];
  baseButton.primary = primary;

  sizes.forEach((size) => {
    let sizeButtons = [];
    states.forEach((state) => {
      let btn = {
        ...baseButton,
        size: size,
        label: primary ? (size === "base" ? "Save" : "Publish") : "Cancel",
      };
      btn[state] = true;
      sizeButtons.push(btn);
    });
    buttons.push({
      size: size,
      buttons: sizeButtons,
    });
  });

  console.log("buttons", buttons);

  return buttons;
};

storiesOf("Components|Buttons", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => (
    <Section>
      <div>
        <h1>Main buttons (primary action)</h1>
        <table style={{ width: 584, borderCollapse: "separate" }}>
          <thead>
            <tr>
              <th>Active</th>
              <th>Hover</th>
              <th>Click</th>
              <th>Disable</th>
            </tr>
          </thead>
          <tbody>
            {Object.values(getButtons(true)).map((btnSize, i) => {
              console.log(btnSize);
              return (
                <tr key={`row${i}`}>
                  {Object.values(btnSize.buttons).map((btn, j) => (
                    <td key={`col${i}${j}`} style={{ paddingBottom: 20 }}>
                      <Button key={`btn${i}${j}`} {...btn} />
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
      <div style={{ marginTop: 47 }}>
        <h1>Main buttons (secondary action)</h1>
        <table style={{ width: 584, borderCollapse: "separate" }}>
          <thead>
            <tr>
              <th>Active</th>
              <th>Hover</th>
              <th>Click</th>
              <th>Disable</th>
            </tr>
          </thead>
          <tbody>
            {Object.values(getButtons(false)).map((btnSize, i) => {
              console.log(btnSize);
              return (
                <tr key={`row${i}`}>
                  {Object.values(btnSize.buttons).map((btn, j) => (
                    <td key={`col${i}${j}`} style={{ paddingBottom: 20 }}>
                      <Button key={`btn${i}${j}`} {...btn} />
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </Section>
  ));
