import React, { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useNavigate, useLocation } from "react-router-dom";
import CatalogItem from "@docspace/components/catalog-item";

import { settingsTree } from "SRC_DIR/utils/settingsTree";
import { getItemByLink } from "SRC_DIR/utils";
import { TSettingsTreeItem } from "SRC_DIR/types/index";

const ArticleBodyContent = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [selectedKey, setSelectedKey] = useState("0");

  useEffect(() => {
    const path = location.pathname;
    const item = getItemByLink(path);
    setSelectedKey(item.key);
  }, []);

  const onClickItem = (item: TSettingsTreeItem) => {
    const path = "/management/" + item.link;
    setSelectedKey(item.key);
    navigate(path);
  };

  const catalogItems = () => {
    const items: Array<React.ReactNode> = [];

    let resultTree = [...settingsTree];

    resultTree.map((item) => {
      items.push(
        <CatalogItem
          key={item.key}
          id={item.key}
          icon={item.icon}
          showText={true}
          text={item.tKey}
          value={item.link}
          isActive={item.key === selectedKey}
          onClick={() => onClickItem(item)}
          folderId={item.id}
        />
      );
    });

    return items;
  };

  const items = catalogItems();

  return <>{items}</>;
};

export default observer(ArticleBodyContent);
