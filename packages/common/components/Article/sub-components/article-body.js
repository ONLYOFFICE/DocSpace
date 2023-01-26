import React from "react";
import Scrollbar from "@docspace/components/scrollbar";
import { classNames } from "@docspace/components/utils/classNames";

const ArticleBody = ({ children, className }) => {
  return (
    <Scrollbar
      className={classNames(className, "article-body__scrollbar")}
      stype="mediumBlack"
    >
      {children}
    </Scrollbar>
  );
};

ArticleBody.displayName = "Body";

export default React.memo(ArticleBody);
