using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! Supply Ports
/* 
Why I need it:
I need to be able to search through all structures that are able to supply a
resource to a rover.

What I need it to do:
I need the port to read the available resources from one or more inventories inside the structure.
I need the port to be able to be disabled dynamically. Likely it will simply be destroyed when all of a machines outputs are connected.
I need hoppers to be able to be prioritised over machines when rovers are searching through ports.
*/

public class SupplyPort
{
    public static List<SupplyPort> Pool = new();

    public Structure parent;

    public SupplyPort(Structure parent)
    {
        Pool.Add(this);
        this.parent = parent;
    } 
}

public class RequestPort
{
    public static List<RequestPort> Pool = new();

    public Structure parent;

    public RequestPort(Structure parent)
    {
        Pool.Add(this);
        this.parent = parent;
    }
}