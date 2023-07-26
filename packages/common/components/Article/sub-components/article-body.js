import React from "react";
import Scrollbar from "@docspace/components/scrollbar";

const ArticleBody = ({ children, className }) => {
  return (
    <Scrollbar
      className="article-body__scrollbar"
      scrollclass="article-scroller"
      stype="mediumBlack"
    >
      {children}
    </Scrollbar>
  );
};

ArticleBody.displayName = "Body";

export default React.memo(ArticleBody);
