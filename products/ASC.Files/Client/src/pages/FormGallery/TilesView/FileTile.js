import React from "react";
import Tile from "./sub-components/Tile";
import { SimpleFilesTileContent } from "./StyledTileView";
import Link from "@appserver/components/link";
import { isDesktop } from "react-device-detect";

const FileTile = (props) => {
  const { item } = props;

  return (
    <div ref={props.selectableRef}>
      <Tile key={item.id} item={item}>
        <SimpleFilesTileContent>
          <Link
            className="item-file-name"
            containerWidth="100%"
            type="page"
            fontWeight="600"
            fontSize={isDesktop ? "13px" : "14px"}
            target="_blank"
            isTextOverflow
          >
            {item.attributes.name_form}
          </Link>
        </SimpleFilesTileContent>
      </Tile>
    </div>
  );
};

export default FileTile;
