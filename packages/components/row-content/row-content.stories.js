import React, { useState } from "react";
import RowContent from "./";
import Link from "../link";
import Checkbox from "../checkbox";
import SendClockReactSvg from "PUBLIC_DIR/images/send.clock.react.svg";
import CatalogSpamReactSvg from "PUBLIC_DIR/images/catalog.spam.react.svg";

export default {
  title: "Components/RowContent",
  component: RowContent,
};

const Template = (args) => {
  const [isChecked, setIsChecked] = useState(false);
  return (
    <>
      <h3>Base demo</h3>
      <div style={{ height: "16px" }}></div>
      <RowContent {...args}>
        <Link type="page" title="Demo" isBold={true} fontSize="15px">
          Demo
        </Link>
        <>
          <SendClockReactSvg size="small" color="#3B72A7" />
          <CatalogSpamReactSvg size="small" color="#3B72A7" />
        </>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link
          containerWidth="160px"
          type="action"
          title="Demo"
          fontSize="12px"
          color="#A3A9AE"
        >
          Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link
          containerWidth="160px"
          type="page"
          title="demo@demo.com"
          fontSize="12px"
          color="#A3A9AE"
        >
          demo@demo.com
        </Link>
      </RowContent>
      <RowContent>
        <Link type="page" title="Demo Demo" isBold={true} fontSize="15px">
          Demo Demo
        </Link>
        <>
          <CatalogSpamReactSvg size="small" color="#3B72A7" />
        </>
        <></>
        <Link
          containerWidth="160px"
          type="action"
          title="Demo Demo"
          fontSize="12px"
          color="#A3A9AE"
        >
          Demo Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link
          containerWidth="160px"
          type="page"
          title="demo.demo@demo.com"
          fontSize="12px"
          color="#A3A9AE"
        >
          demo.demo@demo.com
        </Link>
      </RowContent>
      <RowContent>
        <Link type="page" title="Demo Demo Demo" isBold={true} fontSize="15px">
          Demo Demo Demo
        </Link>
        <></>
        <></>
        <Link
          containerWidth="160px"
          type="action"
          title="Demo Demo Demo"
          fontSize="12px"
          color="#A3A9AE"
        >
          Demo Demo Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link
          containerWidth="160px"
          type="page"
          title="demo.demo.demo@demo.com"
          fontSize="12px"
          color="#A3A9AE"
        >
          demo.demo.demo@demo.com
        </Link>
      </RowContent>
      <RowContent>
        <Link
          type="page"
          title="Demo Demo Demo Demo"
          isBold={true}
          fontSize="15px"
        >
          Demo Demo Demo Demo
        </Link>
        <>
          <SendClockReactSvg size="small" color="#3B72A7" />
        </>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link
          containerWidth="160px"
          type="action"
          title="Demo Demo Demo Demo"
          fontSize="12px"
          color="#A3A9AE"
        >
          Demo Demo Demo Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link
          containerWidth="160px"
          type="page"
          title="demo.demo.demo.demo@demo.com"
          fontSize="12px"
          color="#A3A9AE"
        >
          demo.demo.demo.demo@demo.com
        </Link>
      </RowContent>
      <div style={{ height: "36px" }}></div>
      <h3>Custom elements</h3>
      <div style={{ height: "16px" }}></div>
      <RowContent disableSideInfo={true}>
        <Link type="page" title="John Doe" isBold={true} fontSize="15px">
          John Doe
        </Link>
        <></>
        <Checkbox
          id="1"
          name="sample"
          isChecked={isChecked}
          onChange={(e) => {
            setIsChecked(e.target.checked);
          }}
        />
      </RowContent>
    </>
  );
};

export const basic = Template.bind({});
