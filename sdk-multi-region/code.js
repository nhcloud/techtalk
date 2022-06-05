function resolveConflicts(incomingItem, existingItem, isTombstone, conflictingItems) {
    if (!incomingItem) {
        if (existingItem) {
            __.deleteDocument(existingItem._self, {});
        }
    }
    else if (isTombstone) {
    }
    else {
        if (existingItem) {
            if (incomingItem.metadata.sortableTimestamp > existingItem.metadata.sortableTimestamp) {
                return;
            }
        }
        var i;
        for (i = 0; i < conflictingItems.length; i++) {
            if (incomingItem.metadata.sortableTimestamp > conflictingItems[i].metadata.sortableTimestamp) {
                return;
            }
        }
        delete (conflictingItems, incomingItem, existingItem);
    }
    function delete (documents, incoming, existing)
    {
        if (documents.length > 0) {
            __.deleteDocument(documents[0]._self, {},
                function (err, responseOptions) {
                    documents.shift();
                    delete (documents, incoming, existing);
                }
            );
        }
        else if (existing) {
            __.replaceDocument(existing._self, incoming);
        }
        else {
            __.createDocument(collection.getSelfLink(), incoming);
        }
    }
} 