import React from "react";
import Scrollbar from "@appserver/components/scrollbar";
import LoaderArticleBody from "./article-body-loader";
const ArticleBody = ({ children, isLoading = false }) => {
  return isLoading ? (
    <LoaderArticleBody />
  ) : (
    <Scrollbar className="article-body__scrollbar" stype="mediumBlack">
      {children}
    </Scrollbar>
  );
};

ArticleBody.displayName = "Body";

export default React.memo(ArticleBody);
